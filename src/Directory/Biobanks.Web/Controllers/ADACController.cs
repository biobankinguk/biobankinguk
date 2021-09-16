using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Search.Legacy;
using Biobanks.Identity.Constants;
using Biobanks.Services.Contracts;
using Biobanks.Web.Extensions;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using Microsoft.AspNet.Identity;
using Biobanks.Web.Utilities;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.Home;
using Biobanks.Directory.Data.Constants;
using Biobanks.Web.Models.Search;
using Biobanks.Entities.Data.ReferenceData;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.ApplicationInsights;
using Biobanks.Directory.Services.Constants;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "ADAC")]
    public class ADACController : ApplicationBaseController
    {
        private readonly ICollectionService _collectionService;

        private readonly IReferenceDataService<Funder> _funderService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IEmailService _emailService;
        private readonly IRegistrationDomainService _registrationDomainService;
        private readonly IConfigService _configService;

        private readonly IBiobankIndexService _indexService;

        private readonly ISearchProvider _searchProvider;

        private readonly IMapper _mapper;
        private readonly ITokenLoggingService _tokenLog;

        public ADACController(
            ICollectionService collectionService,
            IReferenceDataService<Funder> funderService,
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IAnalyticsReportGenerator analyticsReportGenerator,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IEmailService emailService,
            IRegistrationDomainService registrationDomainService,
            IConfigService configService,
            IBiobankIndexService indexService,
            ISearchProvider searchProvider,
            IMapper mapper,
            ITokenLoggingService tokenLog)
        {
            _collectionService = collectionService;
            _funderService = funderService;
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _analyticsReportGenerator = analyticsReportGenerator;
            _userManager = userManager;
            _emailService = emailService;
            _registrationDomainService = registrationDomainService;
            _configService = configService;
            _indexService = indexService;
            _searchProvider = searchProvider;
            _mapper = mapper;
            _tokenLog = tokenLog;
        }

        // GET: ADACAdmin
        public ActionResult Index()
        {
            return RedirectToAction("Requests");
        }

        public ActionResult LockedRef()
        {
            return View();
        }

        #region Requests

        public async Task<ActionResult> Requests()
        {
            var bbRequests =
                (await _biobankReadService.ListOpenBiobankRegisterRequestsAsync())
                .Select(x => new BiobankRequestModel
                {
                    RequestId = x.OrganisationRegisterRequestId,
                    BiobankName = x.OrganisationName,
                    UserName = x.UserName,
                    UserEmail = x.UserEmail
                }).ToList();

            var nwRequests =
                (await _biobankReadService.ListOpenNetworkRegisterRequestsAsync())
                .Select(x => new NetworkRequestModel
                {
                    RequestId = x.NetworkRegisterRequestId,
                    NetworkName = x.NetworkName,
                    UserName = x.UserName,
                    UserEmail = x.UserEmail
                }).ToList();

            var model = new RequestsModel
            {
                BiobankRequests = bbRequests,
                NetworkRequests = nwRequests
            };

            return View(model);
        }

        public async Task<ActionResult> AcceptBiobankRequest(int requestId)
        {
            //Let's fetch the request
            var request = await _biobankReadService.GetBiobankRegisterRequestAsync(requestId);
            if (request == null)
            {
                SetTemporaryFeedbackMessage(
                    "That request doesn't exist",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //what if the request is already accepted/declined?
            if (request.AcceptedDate != null || request.DeclinedDate != null)
            {
                SetTemporaryFeedbackMessage(
                    $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} has already been approved or declined.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //try and get the user for the request
            var user = await _userManager.FindByEmailAsync(request.UserEmail);

            // Send email confirming registrations
            if (user is null)
            {
                // Create new user, with new user registration email
                user = new ApplicationUser
                {
                    Email = request.UserEmail,
                    UserName = request.UserEmail,
                    Name = request.UserName
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    return View("GlobalErrors");
                }

                // Send email confirmation of registration
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

                await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

                await _emailService.SendNewUserRegisterEntityAccepted(
                    request.UserEmail,
                    request.UserName,
                    request.OrganisationName,
                    Url.Action("Confirm", "Account",
                        new
                        {
                            userId = user.Id,
                            token = confirmToken
                        },
                        Request.Url.Scheme)
                    );
            }
            else
            {
                // Exisiting user - Send email confirmation of registration
                await _emailService.SendExistingUserRegisterEntityAccepted(
                    request.UserEmail,
                    request.UserName,
                    request.OrganisationName,
                    Url.Action("SwitchToBiobank", "Account",
                        new
                        {
                            id = request.OrganisationRegisterRequestId,
                            newBiobank = true
                        },
                        Request.Url.Scheme)
                );
            }


            //add user to BiobankAdmin role
            await _userManager.AddToRolesAsync(user.Id, Role.BiobankAdmin.ToString());

            //finally, update the request
            request.AcceptedDate = DateTime.Now;
            await _biobankWriteService.UpdateOrganisationRegisterRequestAsync(request);

            //send back, with feedback
            SetTemporaryFeedbackMessage(
                $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} accepted!",
                FeedbackMessageType.Success);

            return RedirectToAction("Requests");
        }

        public async Task<ActionResult> BiobankActivity()
        {
            var biobanks = _mapper.Map<List<BiobankActivityModel>>(await _biobankReadService.GetBiobanksActivityAsync());
            return View(biobanks);
        }

        public async Task<ActionResult> DeclineBiobankRequest(int requestId)
        {
            //Let's fetch the request
            var request = await _biobankReadService.GetBiobankRegisterRequestAsync(requestId);
            if (request == null)
            {
                SetTemporaryFeedbackMessage(
                    "That request doesn't exist",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //what if the request is already accepted/declined?
            if (request.AcceptedDate != null || request.DeclinedDate != null)
            {
                SetTemporaryFeedbackMessage(
                    $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} has already been approved or declined.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //update the request
            request.DeclinedDate = DateTime.Now;
            await _biobankWriteService.UpdateOrganisationRegisterRequestAsync(request);

            //send the requester an email
            await _emailService.SendRegisterEntityDeclined(
                request.UserEmail,
                request.UserName,
                request.OrganisationName);

            //send back, with feedback
            SetTemporaryFeedbackMessage(
                $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} declined!",
                FeedbackMessageType.Success);

            return RedirectToAction("Requests");
        }

        public async Task<ActionResult> AcceptNetworkRequest(int requestId)
        {
            //Let's fetch the request
            var request = await _biobankReadService.GetNetworkRegisterRequestAsync(requestId);
            if (request == null)
            {
                SetTemporaryFeedbackMessage(
                    "That request doesn't exist",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //what if the request is already accepted/declined?
            if (request.AcceptedDate != null || request.DeclinedDate != null)
            {
                SetTemporaryFeedbackMessage(
                    $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} has already been approved or declined.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //try and get the user for the request
            var user = await _userManager.FindByEmailAsync(request.UserEmail);

            //If necessary, create a new user (with no password, so requiring confirmation/reset)
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = request.UserEmail,
                    UserName = request.UserEmail,
                    Name = request.UserName
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View("GlobalErrors");
                }

                //Send email confirmation of registration
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

                await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

                await _emailService.SendNewUserRegisterEntityAccepted(
                    request.UserEmail,
                    request.UserName,
                    request.NetworkName,
                    Url.Action("Confirm", "Account",
                        new
                        {
                            userId = user.Id,
                            token = confirmToken
                        },
                        Request.Url.Scheme));
            }
            else
            {
                //Send email confirmation of registration
                await _emailService.SendExistingUserRegisterEntityAccepted(
                    request.UserEmail,
                    request.UserName,
                    request.NetworkName,
                    Url.Action("SwitchToNetwork", "Account",
                            new
                            {
                                id = request.NetworkRegisterRequestId,
                                newNetwork = true
                            },
                            Request.Url.Scheme)
                    );
            }

            //add user to NetworkAdmin role
            await _userManager.AddToRolesAsync(user.Id, Role.NetworkAdmin.ToString());

            //finally, update the request
            request.AcceptedDate = DateTime.Now;
            await _biobankWriteService.UpdateNetworkRegisterRequestAsync(request);

            //send back, with feedback
            SetTemporaryFeedbackMessage(
                $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} accepted!",
                FeedbackMessageType.Success);

            return RedirectToAction("Requests");
        }

        public async Task<ActionResult> DeclineNetworkRequest(int requestId)
        {
            //Let's fetch the request
            var request = await _biobankReadService.GetNetworkRegisterRequestAsync(requestId);
            if (request == null)
            {
                SetTemporaryFeedbackMessage(
                    "That request doesn't exist",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //what if the request is already accepted/declined?
            if (request.AcceptedDate != null || request.DeclinedDate != null)
            {
                SetTemporaryFeedbackMessage(
                    $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} has already been approved or declined.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Requests");
            }

            //update the request
            request.DeclinedDate = DateTime.Now;
            await _biobankWriteService.UpdateNetworkRegisterRequestAsync(request);

            //send the requester an email
            await _emailService.SendRegisterEntityDeclined(
                request.UserEmail,
                request.UserName,
                request.NetworkName);

            //send back, with feedback
            SetTemporaryFeedbackMessage(
                $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} declined!",
                FeedbackMessageType.Success);

            return RedirectToAction("Requests");
        }

        public async Task<ActionResult> ManualActivation(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null && !user.EmailConfirmed)
            {
                // Generate Token Link
                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var tokenLink = Url.Action("Confirm", "Account",
                    new
                    {
                        userId = user.Id,
                        token = confirmToken
                    },
                    Request.Url.Scheme);

                // Log Token Issuing
                await _tokenLog.TokenIssued(confirmToken, user.Id, "Manual Account Confirmation");

                // Return Link To User
                return Json(new { link = tokenLink }, JsonRequestBehavior.AllowGet);
            }

            return new HttpNotFoundResult();
        }
        #endregion

        #region Biobanks
        public async Task<ActionResult> BiobankAdmin(int id = 0)
        {
            var biobank = await _biobankReadService.GetBiobankByIdAsync(id);

            if (biobank != null)
            {
                var model = _mapper.Map<BiobankModel>(biobank);

                //get the admins
                model.Admins =
                    (await _biobankReadService.ListBiobankAdminsAsync(model.BiobankId)).Select(x => new RegisterEntityAdminModel
                    {
                        UserId = x.Id,
                        UserFullName = x.Name,
                        UserEmail = x.Email,
                        EmailConfirmed = x.EmailConfirmed
                    }).ToList();

                return View(model);
            }
            else
                return HttpNotFound();
        }

        public async Task<ActionResult> DeleteAdmin(int biobankId, string biobankUserId)
        {
            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            var userFullName =
                (await _biobankReadService.ListBiobankAdminsAsync(biobankId)).Select(x => new RegisterEntityAdminModel
                {
                    UserId = x.Id,
                    UserFullName = x.Name,
                    UserEmail = x.Email,
                    EmailConfirmed = x.EmailConfirmed
                }).SingleOrDefault(x => x.UserId == biobankUserId).UserFullName;

            //remove them from the network
            await _biobankWriteService.RemoveUserFromBiobankAsync(biobankUserId, biobankId);

            //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
            await _userManager.RemoveFromRolesAsync(biobankUserId, Role.BiobankAdmin.ToString());

            SetTemporaryFeedbackMessage($"{userFullName} has been removed from the admins!", FeedbackMessageType.Success);

            return RedirectToAction("BiobankAdmin", new { id = biobankId });
        }

        public async Task<ActionResult> Biobanks()
        {
            var allBiobanks =
                (await _biobankReadService.ListBiobanksAsync()).ToList();

            var biobanks = allBiobanks.Select(x => new BiobankModel
            {
                BiobankId = x.OrganisationId,
                BiobankExternalId = x.OrganisationExternalId,
                Name = x.Name,
                IsSuspended = x.IsSuspended,
                ContactEmail = x.ContactEmail
            }).ToList();

            foreach (var biobank in biobanks)
            {
                //get the admins
                biobank.Admins =
                    (await _biobankReadService.ListBiobankAdminsAsync(biobank.BiobankId)).Select(x => new RegisterEntityAdminModel
                    {
                        UserId = x.Id,
                        UserFullName = x.Name,
                        UserEmail = x.Email,
                        EmailConfirmed = x.EmailConfirmed
                    }).ToList();
            }

            var model = new BiobanksModel
            {
                Biobanks = biobanks
            };

            return View(model);
        }

        public async Task<ActionResult> InviteAdminAjax(int biobankId)
        {
            var bb = await _biobankReadService.GetBiobankByIdAsync(biobankId);

            return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
            {
                Entity = bb.Name,
                EntityName = "biobank",
                ControllerName = "ADAC"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> InviteAdminAjax(InviteRegisterEntityAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .Where(x => x.Errors.Count > 0)
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()
                });
            }

            var biobankId = (await _biobankReadService.GetBiobankByNameAsync(model.Entity)).OrganisationId;
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                //User doesn't exist; add a new one
                //Create a new user, no password at this time (so they don't really function yet)
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    //send email to confirm account
                    var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

                    await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

                    await _emailService.SendNewUserRegisterEntityAdminInvite(
                        model.Email,
                        model.Name,
                        model.Entity,
                        Url.Action("Confirm", "Account",
                            new
                            {
                                userId = user.Id,
                                token = confirmToken
                            },
                            Request.Url.Scheme));
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        errors = result.Errors.ToArray()
                    });
                }

                //Maybe there should be an auth (or shared) method for all this
            }
            else
            {
                //send email to inform the existing user they've been added as an admin
                await _emailService.SendExistingUserRegisterEntityAdminInvite(
                    model.Email,
                    model.Name,
                    model.Entity,
                    Url.Action("BiobankAdmin", "Adac", new { id = biobankId }, Request.Url.Scheme));
            }

            //Add the user/biobank relationship
            await _biobankWriteService.AddUserToBiobankAsync(user.Id, biobankId);

            //add user to BiobankAdmin role
            await _userManager.AddToRolesAsync(user.Id, Role.BiobankAdmin.ToString()); //what happens if they're already in the role?

            //return success, and enough user details for adding to the viewmodel's list
            return Json(new
            {
                success = true,
                userId = user.Id,
                name = user.Name,
                email = user.Email,
                emailConfirmed = user.EmailConfirmed
            });
        }

        [HttpGet]
        public async Task<ActionResult> DeleteBiobank(int id)
        {
            var biobank = await _biobankReadService.GetBiobankByIdAsync(id);

            if (biobank != null) return View(_mapper.Map<BiobankModel>(biobank));

            SetTemporaryFeedbackMessage("The selected biobank could not be found.", FeedbackMessageType.Danger);
            return RedirectToAction("Biobanks");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteBiobank(BiobankModel model)
        {
            try
            {
                // remove the biobank itself
                var biobank = await _biobankReadService.GetBiobankByIdAsync(model.BiobankId);
                var usersInBiobank = await _biobankReadService.ListSoleBiobankAdminIdsAsync(model.BiobankId);
                await _biobankWriteService.DeleteBiobankAsync(model.BiobankId);

                // remove admin role from users who had admin role only for this biobank
                foreach (var user in usersInBiobank)
                {
                    await _userManager.RemoveFromRolesAsync(user.Id, Role.BiobankAdmin.ToString());
                }

                //remove biobank registration request to allow re-registration 
                var biobankRequest = await _biobankReadService.GetBiobankRegisterRequestByOrganisationNameAsync(biobank.Name);
                await _biobankWriteService.DeleteRegisterRequestAsync(biobankRequest);
                SetTemporaryFeedbackMessage($"{biobank.Name} and its associated data has been deleted.", FeedbackMessageType.Success);
            }
            catch
            {

                SetTemporaryFeedbackMessage("The selected biobank could not be deleted.", FeedbackMessageType.Danger);
            }

            return RedirectToAction("Biobanks");
        }

        public async Task<ActionResult> SuspendBiobank(int id)
        {
            try
            {
                var biobank = await _biobankWriteService.SuspendBiobankAsync(id);
                if (biobank.IsSuspended)
                    SetTemporaryFeedbackMessage($"{biobank.Name} has been suspended.", FeedbackMessageType.Success);
            }
            catch
            {
                SetTemporaryFeedbackMessage("The selected biobank could not be suspended.", FeedbackMessageType.Danger);
            }

            return RedirectToAction($"BiobankAdmin", new { id = id });
        }

        public async Task<ActionResult> UnsuspendBiobank(int id)
        {
            try
            {
                var biobank = await _biobankWriteService.UnsuspendBiobankAsync(id);
                if (!biobank.IsSuspended)
                    SetTemporaryFeedbackMessage($"{biobank.Name} has been unsuspended.", FeedbackMessageType.Success);
            }
            catch
            {
                SetTemporaryFeedbackMessage("The selected biobank could not be unsuspended.", FeedbackMessageType.Danger);
            }

            return RedirectToAction($"BiobankAdmin", new { id = id });
        }

        public async Task<ActionResult> GenerateResetLinkAjax(string biobankUserId, string biobankUsername)
        {
            // Get the reset token
            var resetToken = await _biobankReadService.GetUnusedTokenByUser(biobankUserId);
            await _tokenLog.PasswordTokenIssued(resetToken.ToString(), biobankUserId);

            // Generate the reset URL
            var url = Url.Action("ResetPassword", "Account",
                new { userId = biobankUserId, token = resetToken },
                Request.Url.Scheme);

            return PartialView("_ModalResetPassword", new ResetPasswordEntityModel
            {
                ResetLink = url,
                UserName = biobankUsername
            });
        }

        #endregion

        #region Funders

        public async Task<ActionResult> Funders()
        {
            return View(
                (await _funderService.List())
                    .Select(x =>
                        new FunderModel
                        {
                            FunderId = x.Id,
                            Name = x.Value
                        })
                    .ToList()
                );
        }

        [HttpGet]
        public async Task<ActionResult> DeleteFunder(int id)
            => View(await _funderService.Get(id));
      
        [HttpPost]
        public async Task<ActionResult> DeleteFunder(FunderModel model)
        {
            try
            {
                await _funderService.Delete(model.FunderId);

                SetTemporaryFeedbackMessage($"{model.Name} and its associated data has been deleted.", FeedbackMessageType.Success);
            }
            catch
            {

                SetTemporaryFeedbackMessage("The selected funder could not be deleted.", FeedbackMessageType.Danger);
            }

            return RedirectToAction("Funders");
        }

        [HttpPost]
        public async Task<JsonResult> EditFunderAjax(FunderModel model)
        {
            //If this description is valid, it already exists
            if (await _funderService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That funder name is already in use. Funder names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _funderService.Update(new Funder
            {
                Id = model.FunderId,
                Value = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult EditFunderSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The funder \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Funders");
        }

        [HttpPost]
        public async Task<JsonResult> AddFunderAjax(FunderModel model)
        {
            //If this description is valid, it already exists
            if (await _funderService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That funder name is already in use. Funder names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _funderService.Add(new Funder
            {
                Value = model.Name,
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult AddFunderSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The funder \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Funders");
        }

        #endregion 

        #region Networks

        public async Task<ActionResult> Networks()
        {
            var allNetworks =
                (await _biobankReadService.ListNetworksAsync()).ToList();

            var networks = allNetworks.Select(x => new NetworkModel
            {
                NetworkId = x.NetworkId,
                Name = x.Name
            }).ToList();

            foreach (var network in networks)
            {
                //get the admins
                network.Admins =
                    (await _biobankReadService.ListNetworkAdminsAsync(network.NetworkId)).Select(x => new RegisterEntityAdminModel
                    {
                        UserId = x.Id,
                        UserFullName = x.Name,
                        UserEmail = x.Email,
                        EmailConfirmed = x.EmailConfirmed
                    }).ToList();
            }

            var model = new NetworksModel
            {
                Networks = networks
            };

            return View(model);
        }

        #endregion

        #region Historical

        public async Task<ActionResult> Historical()
        {
            //get both network and biobank historical requests
            //and convert them to the viewmodel format
            var bbRequests = (await _biobankReadService.ListHistoricalBiobankRegisterRequestsAsync())
                .Select(x =>

                    Task.Run(async () =>
                    {
                        string action;
                        DateTime date;
                        GetHistoricalRequestActionDate(x.DeclinedDate, x.AcceptedDate, out action, out date);
                        var user = await _userManager.FindByEmailAsync(x.UserEmail);

                        return new HistoricalRequestModel
                        {
                            UserName = x.UserName,
                            UserEmail = x.UserEmail,
                            EntityName = x.OrganisationName,
                            Action = action,
                            Date = date,
                            UserEmailConfirmed = user?.EmailConfirmed ?? false,
                            ResultingOrgExternalId = x.OrganisationExternalId
                        };
                    }).Result

                ).ToList();

            var nwRequests = (await _biobankReadService.ListHistoricalNetworkRegisterRequestsAsync())
                .Select(x =>

                    Task.Run(async () =>
                    {
                        string action;
                        DateTime date;
                        GetHistoricalRequestActionDate(x.DeclinedDate, x.AcceptedDate, out action, out date);
                        var user = await _userManager.FindByEmailAsync(x.UserEmail);

                        return new HistoricalRequestModel
                        {
                            UserName = x.UserName,
                            UserEmail = x.UserEmail,
                            EntityName = x.NetworkName,
                            Action = action,
                            Date = date,
                            UserEmailConfirmed = user?.EmailConfirmed ?? false
                        };
                    }).Result

                ).ToList();

            var model = new HistoricalModel
            {
                HistoricalRequests = bbRequests.Concat(nwRequests).ToList()
            };

            return View(model);
        }

        private static void GetHistoricalRequestActionDate(DateTime? declineDate, DateTime? acceptedDate, out string action, out DateTime date)
        {
            //check it is actually historical
            if (declineDate == null && acceptedDate == null) throw new ApplicationException();

            date = (declineDate ?? acceptedDate).Value;
            action = (declineDate != null) ? "Declined" : "Accepted";
        }

        #endregion

        #region Analytics
        public async Task<ActionResult> Analytics(int year = 0, int endQuarter = 0, int reportPeriod = 0)
        {
            //If turned off in site config
            if (await _configService.GetFlagConfigValue(ConfigKey.DisplayAnalytics) == false)
                return RedirectToAction("LockedRef");

            //set default options
            if (year == 0)
                year = DateTime.Today.Year;
            if (endQuarter == 0)
                endQuarter = ((DateTime.Today.Month + 2) / 3);
            if (reportPeriod == 0)
                reportPeriod = 5;

            try
            {
                var model = _mapper.Map<DirectoryAnalyticReport>(await _analyticsReportGenerator.GetDirectoryReport(year, endQuarter, reportPeriod));
                return View(model);
            }
            catch (Exception e)
            {
                var message = e switch
                {
                    JsonSerializationException _ => "The API Response Body could not be processed.",
                    HttpRequestException _ => "The API Request failed.",
                    _ => "An unknown error occurred and has been logged."
                };

                var outer = new Exception(message, e);

                // Log Error via Application Insights
                var ai = new TelemetryClient();
                ai.TrackException(outer);

                ModelState.AddModelError(string.Empty, outer);
                return View(new DirectoryAnalyticReport());
            }
        }
        #endregion

        #region Reference Datasets

        public ActionResult AddRefDataSuccessFeedbackAjax(RefDataFeedbackModel feedback)
        {
            SetTemporaryFeedbackMessage($"The {feedback.RefDataType.ToLower()} \"{feedback.Name}\" has been added successfully.", FeedbackMessageType.Success);
            return Redirect(feedback.RedirectUrl);
        }
        public ActionResult EditRefDataSuccessFeedbackAjax(RefDataFeedbackModel feedback)
        {
            SetTemporaryFeedbackMessage($"The {feedback.RefDataType.ToLower()} \"{feedback.Name}\" has been edited successfully.", FeedbackMessageType.Success);
            return Redirect(feedback.RedirectUrl);
        }
        public ActionResult DeleteRefDataSuccessFeedbackAjax(RefDataFeedbackModel feedback)
        {
            SetTemporaryFeedbackMessage($"The {feedback.RefDataType.ToLower()} \"{feedback.Name}\" has been deleted successfully.", FeedbackMessageType.Success);
            return Redirect(feedback.RedirectUrl);
        }

        #region RefData: Access Conditions
        public async Task<ActionResult> AccessConditions()
        {
            var models = (await _biobankReadService.ListAccessConditionsAsync())
            .Select(x =>
                Task.Run(async () => new ReadAccessConditionsModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder,
                    AccessConditionCount = await _biobankReadService.GetAccessConditionsCount(x.Id),
                }
                )
                .Result
            )
            .ToList();

            return View(new AccessConditionsModel
            {
                AccessConditions = models
            });
        }
        #endregion

        #region RefData: Age Ranges
        public async Task<ActionResult> AgeRanges()
        {
            var models = (await _biobankReadService.ListAgeRangesAsync())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetAgeRangeUsageCount(x.Id),
                        LowerBound = ConvertFromIsoDuration(x.LowerBound),
                        UpperBound = ConvertFromIsoDuration(x.UpperBound)
                    })
                    .Result
                )
                .ToList();

            return View(new AgeRangesModel
            {
                AgeRanges = models
            });

        }

        // Converting from Iso Duration to 'plain text' - e.g P6M -> 6 Months
        private string ConvertFromIsoDuration(string bound)
        {
            var dict = new Dictionary<string, string>
            {
                { "D", "Days" },
                { "M", "Months" },
                { "Y", "Years" }
            };

            var converted = string.IsNullOrEmpty(bound) ? "" : bound.Replace("P", "").Replace(bound.Last().ToString(), " " + dict[bound.Last().ToString()]);

            return converted;
        }


        #endregion
        #region RefData: AssociatedDataProcurementTimeFrame
        public async Task<ActionResult> AssociatedDataProcurementTimeFrame()
        {
            return View(new Models.ADAC.AssociatedDataProcurementTimeFrameModel
            {
                AssociatedDataProcurementTimeFrameModels = (await _biobankReadService.ListAssociatedDataProcurementTimeFrames())
                    .Select(x =>

                Task.Run(async () => new ReadAssociatedDataProcurementTimeFrameModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    DisplayName = x.DisplayValue,
                    CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }

        #endregion

        #region RefData: AnnualStatistics
        public async Task<ActionResult> AnnualStatistics()
        {
            var groups = (await _biobankReadService.ListAnnualStatisticGroupsAsync())
                .Select(x => new AnnualStatisticGroupModel
                {
                    AnnualStatisticGroupId = x.Id,
                    Name = x.Value,
                })
                .ToList();

            var models = (await _biobankReadService.ListAnnualStatisticsAsync())
                .Select(x =>
                    Task.Run(async () => new AnnualStatisticModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        UsageCount = await _biobankReadService.GetAnnualStatisticUsageCount(x.Id),
                        AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                        AnnualStatisticGroupName = groups.Where(y => y.AnnualStatisticGroupId == x.AnnualStatisticGroupId).FirstOrDefault()?.Name,
                    })
                    .Result
                )
                .ToList();

            return View(new AnnualStatisticsModel
            {
                AnnualStatistics = models,
                AnnualStatisticGroups = groups
            });

        }

        #endregion

        #region RefData: Material Types
        public async Task<ActionResult> MaterialTypes()
        {
            var materialTypes = await _biobankReadService.ListMaterialTypesAsync();

            return View(new MaterialTypesModel
            {
                MaterialTypes = materialTypes.Select(x => Task.Run(
                    async () => new ReadMaterialTypeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        MaterialTypeGroups = x.MaterialTypeGroups.Select(x => x.Value),
                        MaterialDetailCount = await _biobankReadService.GetMaterialTypeMaterialDetailCount(x.Id),
                        UsedByExtractionProcedures = await _biobankReadService.IsMaterialTypeAssigned(x.Id)

                    }))
                .Select(x => x.Result)
                .ToList()
            });
        }
        #endregion

        #region RefData: Material Type Groups
        public async Task<ActionResult> MaterialTypeGroups()
        {
            var materialTypes = await _biobankReadService.ListMaterialTypeGroupsAsync();

            return View(materialTypes.Select(x => new MaterialTypeGroupModel
            {
                Id = x.Id,
                Description = x.Value,
                MaterialTypes = x.MaterialTypes.Select(x => x.Value),
                MaterialTypeCount = x.MaterialTypes.Count()
            }));
        }
        #endregion

        #region RefData: Disease Status
        public async Task<ActionResult> DiseaseStatuses()
        {
            var diseaseTerms = await _biobankReadService.ListOntologyTerms(tags: new List<string>
            {
                SnomedTags.Disease
            });

            return View(diseaseTerms.Select(x =>

                Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    CollectionCapabilityCount = await _biobankReadService.GetOntologyTermCollectionCapabilityCount(x.Id),
                    OtherTerms = x.OtherTerms
                })
                .Result
            ));
        }

        public async Task<ActionResult> PagingatedDiseaseStatuses(int draw, int start, int length, IDictionary<string, string> search)
        {
            // Select Search By Value
            var searchValue = search.TryGetValue("value", out var s) ? s : "";
            var tags = new List<string> { SnomedTags.Disease };

            // Get Disease Statuses
            var diseaseTerms = await _biobankReadService.PaginateOntologyTerms(start, length, searchValue, tags);
            var filteredCount = await _biobankReadService.CountOntologyTerms(description: searchValue, tags: tags);
            var totalCount = await _biobankReadService.CountOntologyTerms(tags: tags);

            var data = diseaseTerms.Select(x =>
                Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    OtherTerms = x.OtherTerms,
                    DisplayOnDirectory = x.DisplayOnDirectory,
                    CollectionCapabilityCount = await _biobankReadService.GetOntologyTermCollectionCapabilityCount(x.Id)
                })
                .Result
            );

            return Json(new
            {
                draw,
                data,
                recordsTotal = totalCount,
                recordsFiltered = filteredCount
            },
            JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RefData: Collection Percentages
        public async Task<ActionResult> CollectionPercentages()
        {
            var models = (await _biobankReadService.ListCollectionPercentagesAsync())
                .Select(x =>
                    Task.Run(async () => new CollectionPercentageModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetCollectionPercentageUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();
            if (await _configService.GetFlagConfigValue("site.display.preservation.percent") == true)
            {
                return View(new CollectionPercentagesModel()
                {
                    CollectionPercentages = models
                });
            }
            else
            {
                return RedirectToAction("LockedRef");
            }
        }

        #endregion

        #region RefData: Donor Counts

        public async Task<ActionResult> DonorCounts()
        {
            var models = (await _biobankReadService.ListDonorCountsAsync(true))
                .Select(x =>
                    Task.Run(async () => new DonorCountModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetDonorCountUsageCount(x.Id)
                    })
                        .Result
                )
                .ToList();

            return View(new DonorCountsModel()
            {
                DonorCounts = models
            });
        }
        #endregion

        #region RefData: Collection Type

        public async Task<ActionResult> CollectionType()
        {
            return View(new Models.ADAC.CollectionTypeModel
            {
                CollectionTypes = (await _biobankReadService.ListCollectionTypesAsync())
                     .Select(x =>

                 Task.Run(async () => new ReadCollectionTypeModel
                 {
                     Id = x.Id,
                     Description = x.Value,
                     CollectionCount = await _biobankReadService.GetCollectionTypeCollectionCount(x.Id),
                     SortOrder = x.SortOrder
                 }).Result)

                     .ToList()
            });
        }
        #endregion

        #region RefData: Storage Temperature

        public async Task<ActionResult> StorageTemperatures()
        {
            var models = (await _biobankReadService.ListStorageTemperaturesAsync())
                .Select(x =>
                    new StorageTemperatureModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                    }
                )
                .ToList();

            // Fetch Sample Set Count and whether a Preservation Type is using this storage temperature
            foreach (var model in models)
            {
                model.SampleSetsCount = await _biobankReadService.GetStorageTemperatureUsageCount(model.Id);
                model.UsedByPreservationTypes = await _biobankReadService.IsStorageTemperatureAssigned(model.Id);
            }

            return View(new StorageTemperaturesModel
            {
                StorageTemperatures = models
            });
        }
        #endregion

        #region RefData: Preservation Type

        public async Task<ActionResult> PreservationTypes()
        {
            var models = (await _biobankReadService.ListPreservationTypesAsync())
                .Select(x =>
                    new PreservationTypeModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                        StorageTemperatureId = x.StorageTemperatureId,
                        StorageTemperatureName = x.StorageTemperature?.Value ?? ""
                    }
                )
                .ToList();

            // Fetch Sample Set Count
            foreach (var model in models)
            {
                model.PreservationTypeCount = await _biobankReadService.GetPreservationTypeUsageCount(model.Id);
            }

            return View(new PreservationTypesModel
            {
                PreservationTypes = models,
                StorageTemperatures = await _biobankReadService.ListStorageTemperaturesAsync()
            });
        }

        #endregion

        #region RefData: Assocaited Data Types

        public async Task<ActionResult> AssociatedDataTypes()
        {
            var groups = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync())
                .Select(x => new AssociatedDataTypeGroupModel
                {
                    AssociatedDataTypeGroupId = x.Id,
                    Name = x.Value,
                })
                .ToList();
            var model = (await _biobankReadService.ListAssociatedDataTypesAsync()).Select(x =>

                    Task.Run(async () => new AssociatedDataTypeModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        Message = x.Message,
                        CollectionCapabilityCount =
                            await _biobankReadService.GetAssociatedDataTypeCollectionCapabilityCount(x.Id),
                        AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                        AssociatedDataTypeGroupName = groups
                            .Where(y => y.AssociatedDataTypeGroupId == x.AssociatedDataTypeGroupId).FirstOrDefault()
                            ?.Name,

                    }).Result)

                .ToList();


            return View(new AssociatedDataTypesModel
            {
                AssociatedDataTypes = model,
                AssociatedDataTypeGroups = groups
            });
        }


        #endregion

        #region RefData: Associated Data Type Groups
        public async Task<ActionResult> AssociatedDataTypeGroups()
        {
            return View(new AssociatedDataTypesGroupModel
            {
                AssociatedDataTypeGroups = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync())
                    .Select(x =>

                        Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                        {
                            AssociatedDataTypeGroupId = x.Id,
                            Name = x.Value,
                            AssociatedDataTypeGroupCount =
                                await _biobankReadService.GetAssociatedDataTypeGroupCount(x.Id)
                        }).Result)

                    .ToList()
            });
        }

        #endregion

        #region RefData: Consent Restrictions
        public async Task<ActionResult> ConsentRestriction()
        {
            return View(new Models.ADAC.ConsentRestrictionModel
            {
                ConsentRestrictions = (await _biobankReadService.ListConsentRestrictionsAsync())
                    .Select(x =>

                        Task.Run(async () => new ReadConsentRestrictionModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            CollectionCount = await _biobankReadService.GetConsentRestrictionCollectionCount(x.Id),
                            SortOrder = x.SortOrder
                        }).Result)

                    .ToList()
            });
        }

        #endregion

        #region RefData: Collection Status
        public async Task<ActionResult> CollectionStatus()
        {
            return View(new Models.ADAC.CollectionStatusModel
            {
                CollectionStatuses = (await _biobankReadService.ListCollectionStatusesAsync())
                    .Select(x =>

                Task.Run(async () => new ReadCollectionStatusModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _biobankReadService.GetCollectionStatusCollectionCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }
        #endregion 

        #region RefData: Annual Statistic Groups
        public async Task<ActionResult> AnnualStatisticGroups()
        {
            return View(new AnnualStatisticGroupsModel
            {
                AnnualStatisticGroups = (await _biobankReadService.ListAnnualStatisticGroupsAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadAnnualStatisticGroupModel
                    {
                        AnnualStatisticGroupId = x.Id,
                        Name = x.Value,
                        AnnualStatisticGroupCount = await _biobankReadService.GetAnnualStatisticAnnualStatisticGroupCount(x.Id)
                    }).Result)

                    .ToList()
            });
        }

        #endregion

        #region RefData: Sample Collection Mode
        public async Task<ActionResult> SampleCollectionModes()
        {
            var models = (await _biobankReadService.ListSampleCollectionModeAsync())
                .Select(x =>
                    Task.Run(async () => new SampleCollectionModeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetSampleCollectionModeUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return View(new SampleCollectionModesModel()
            {
                SampleCollectionModes = models
            });
        }
        #endregion

        #region RefData: Sexes
        public async Task<ActionResult> Sexes()
        {
            return View(new SexesModel
            {
                Sexes = (await _biobankReadService.ListSexesAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadSexModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SexCount = await _biobankReadService.GetSexCount(x.Id),
                        SortOrder = x.SortOrder
                    }).Result)

                    .ToList()
            });
        }
        #endregion

        #region RefData: Country
        public async Task<ActionResult> Country()
        {
            return View(new Models.ADAC.CountryModel
            {
                Countries = (await _biobankReadService.ListCountriesAsync())
                     .Select(x =>

                     Task.Run(async () => new ReadCountryModel
                     {
                         Id = x.Id,
                         Name = x.Value,
                         CountyOrganisationCount = await _biobankReadService.GetCountryCountyOrganisationCount(x.Id)
                     }).Result)

                     .ToList()
            });
        }
        #endregion

        #region RefData: County
        public async Task<ActionResult> County()
        {
            if (await _configService.GetFlagConfigValue("site.display.counties") == true)
            {
                var countries = await _biobankReadService.ListCountriesAsync();

                return View(
                    new CountiesModel
                    {
                        Counties = countries.ToDictionary(
                            x => x.Value,
                            x => x.Counties.Select(county =>
                                Task.Run(async () =>
                                    new CountyModel
                                    {
                                        Id = county.Id,
                                        CountryId = x.Id,
                                        Name = county.Value,
                                        CountyUsageCount = await _biobankReadService.GetCountyUsageCount(county.Id)
                                    }
                                 )
                                .Result
                            )
                        )
                    }
                );
            }
            else
            {
                return RedirectToAction("LockedRef");
            }
        }

        #endregion

        #region RefData: Sop Status
        public async Task<ActionResult> SopStatus()
        {
            var models = (await _biobankReadService.ListSopStatusesAsync())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetSopStatusUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return View(new SopStatusesModel()
            {
                SopStatuses = models
            });
        }

        #endregion

        #region RefData: Registration Reason
        public async Task<ActionResult> RegistrationReason()
        {
            return View(new Models.ADAC.RegistrationReasonModel
            {
                RegistrationReasons = (await _biobankReadService.ListRegistrationReasonsAsync())
                    .Select(x =>

                        Task.Run(async () => new ReadRegistrationReasonModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            OrganisationCount = await _biobankReadService.GetRegistrationReasonOrganisationCount(x.Id),
                        }).Result)

                    .ToList()
            });
        }

        #endregion

        #region RefData: Macroscopic Assessment
        public async Task<ActionResult> MacroscopicAssessments()
        {
            var models = (await _biobankReadService.ListMacroscopicAssessmentsAsync())
                .Select(x =>
                    Task.Run(async () => new MacroscopicAssessmentModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetMacroscopicAssessmentUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return View(new MacroscopicAssessmentsModel()
            {
                MacroscopicAssessments = models
            });
        }
        #endregion

        #region RefData: Service Offerings

        public async Task<ActionResult> ServiceOffering()
        {
            return View(new Models.ADAC.ServiceOfferingModel
            {
                ServiceOfferings = (await _biobankReadService.ListServiceOfferingsAsync())
                    .Select(x =>

                Task.Run(async () => new ReadServiceOfferingModel
                {
                    Id = x.Id,
                    Name = x.Value,
                    OrganisationCount = await _biobankReadService.GetServiceOfferingOrganisationCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }

        #endregion

        #region RefData: Extraction Procedure
        public async Task<ActionResult> ExtractionProcedure()
        {
            var ExtractionProcedures = (await _biobankReadService.ListOntologyTerms(tags: new List<string>
                {
                    SnomedTags.ExtractionProcedure
                }));
            return View(new ExtractionProceduresModel
            {
                ExtractionProcedures = (await _biobankReadService.ListOntologyTerms(tags: new List<string>
                {
                    SnomedTags.ExtractionProcedure
                }))
                .Select(x =>

                Task.Run(async () => new ReadExtractionProcedureModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    MaterialDetailsCount = await _biobankReadService.GetExtractionProcedureMaterialDetailsCount(x.Id),
                    OtherTerms = x.OtherTerms,
                    MaterialTypeIds = x.MaterialTypes.Select(x=>x.Id).ToList(),
                    DisplayOnDirectory = x.DisplayOnDirectory
                })
                .Result
            ).ToList(),
                MaterialTypes = await _biobankReadService.ListMaterialTypesAsync()
            });
        }
        #endregion

        #endregion

        #region Site Configuration

        #region Homepage Config
        public ActionResult HomepageConfig()
        {
            return View(new HomepageContentModel
            {
                Title = Config.Get(ConfigKey.HomepageTitle, ""),
                SearchTitle = Config.Get(ConfigKey.HomepageSearchTitle, ""),
                SearchSubTitle = Config.Get(ConfigKey.HomepageSearchSubTitle, ""),
                ResourceRegistration = Config.Get(ConfigKey.HomepageResourceRegistration, ""),
                NetworkRegistration = Config.Get(ConfigKey.HomepageNetworkRegistration, ""),
                RequireSamplesCollected = Config.Get(ConfigKey.HomepageSearchRadioSamplesCollected, ""),
                AccessExistingSamples = Config.Get(ConfigKey.HomepageSearchRadioAccessSamples, "")
            });
        }

        [HttpPost]
        public ActionResult HomepageConfig(HomepageContentModel homepage)
            => View(homepage);

        [HttpPost]
        public async Task<ActionResult> SaveHomepageConfig(HomepageContentModel homepage)
        {
            await _configService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.HomepageTitle, Value = homepage.Title ?? "" },
                    new Config { Key = ConfigKey.HomepageSearchTitle, Value = homepage.SearchTitle ?? "" },
                    new Config { Key = ConfigKey.HomepageSearchSubTitle, Value = homepage.SearchSubTitle ?? "" },
                    new Config { Key = ConfigKey.HomepageResourceRegistration, Value = homepage.ResourceRegistration ?? "" },
                    new Config { Key = ConfigKey.HomepageNetworkRegistration, Value = homepage.NetworkRegistration ?? "" },
                    new Config { Key = ConfigKey.HomepageSearchRadioSamplesCollected, Value = homepage.RequireSamplesCollected ?? ""},
                    new Config { Key = ConfigKey.HomepageSearchRadioAccessSamples, Value = homepage.AccessExistingSamples ?? "" }
                }
            );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;

            SetTemporaryFeedbackMessage("Homepage configuration saved successfully.", FeedbackMessageType.Success);

            return Redirect("HomepageConfig");
        }

        public ActionResult HomepageConfigPreview()
            => Redirect("HomepageConfig");

        [HttpPost]
        public ActionResult HomepageConfigPreview(HomepageContentModel homepage)
            => View("HomepageConfigPreview", homepage);
        #endregion

        #region Termpage Config
        public ActionResult TermpageConfig()
        {
            return View(new TermPageModel
            {
                TermpageContentModel = new TermpageContentModel
                {
                    PageInfo = Config.Get(ConfigKey.TermpageInfo, "")
                }
            });
        }

        [HttpPost]
        public ActionResult TermpageConfig(TermpageContentModel termpage)
        {
            return View(new TermPageModel
            {
                TermpageContentModel = termpage
            });
        }


        [HttpPost]
        public async Task<ActionResult> SaveTermpageConfig(TermpageContentModel termpage)
        {
            await _configService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.TermpageInfo, Value = termpage.PageInfo ?? "" }
                }
            );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;

            SetTemporaryFeedbackMessage("Term page configuration saved successfully.", FeedbackMessageType.Success);

            return Redirect("TermpageConfig");
        }

        public ActionResult TermpageConfigPreview()
            => Redirect("TermpageConfig");

        [HttpPost]
        public async Task<ActionResult> TermpageConfigPreview(TermpageContentModel termpage)
        {
            // Populate Snomed Terms for Preview View
            var collections = await _collectionService.List();

            var ontologyTerms = collections
                .Where(x => x.SampleSets.Any())
                .Select(x => x.OntologyTerm)
                .Distinct();

            // Find CollectionCapabilityCount For Each OntologyTerm
            var ontologyTermModels = ontologyTerms.Select(x =>

                Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    CollectionCapabilityCount = await _biobankReadService.GetOntologyTermCollectionCapabilityCount(x.Id),
                    OtherTerms = x.OtherTerms
                })
                .Result
            );

            return View("TermpageConfigPreview", new TermPageModel
            {
                OntologyTermsModel = ontologyTermModels,
                TermpageContentModel = termpage
            });
        }


        #endregion

        #region Register Biobank and Network Pages Config
        public ActionResult RegisterPagesConfig()
        {
            return View(new RegisterConfigModel
            {
                BiobankTitle = Config.Get(ConfigKey.RegisterBiobankTitle, ""),
                BiobankDescription = Config.Get(ConfigKey.RegisterBiobankDescription, ""),
                NetworkTitle = Config.Get(ConfigKey.RegisterNetworkTitle, ""),
                NetworkDescription = Config.Get(ConfigKey.RegisterNetworkDescription, ""),
                EnableRegistrationHelpUrl = Config.Get(ConfigKey.EnableRegisterRegistrationHelpUrl, ""),
                RegistrationHelpUrl = Config.Get(ConfigKey.RegisterRegistrationHelpUrl, ""),
                RegistrationEmails = Config.Get(ConfigKey.RegistrationEmails, "")
            });
        }


        [HttpPost]
        public async Task<ActionResult> SaveRegisterPagesConfig(RegisterConfigModel registerConfigModel)
        {
            await _configService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.RegisterBiobankTitle, Value = registerConfigModel.BiobankTitle ?? ""},
                    new Config { Key = ConfigKey.RegisterBiobankDescription, Value = registerConfigModel.BiobankDescription ?? "" },
                    new Config { Key = ConfigKey.RegisterNetworkTitle, Value = registerConfigModel.NetworkTitle ?? ""},
                    new Config { Key = ConfigKey.RegisterNetworkDescription, Value = registerConfigModel.NetworkDescription ?? "" },
                    new Config { Key = ConfigKey.EnableRegisterRegistrationHelpUrl, Value = registerConfigModel.EnableRegistrationHelpUrl ?? "" },
                    new Config { Key = ConfigKey.RegisterRegistrationHelpUrl, Value = registerConfigModel.RegistrationHelpUrl ?? "" },
                    new Config { Key = ConfigKey.RegistrationEmails, Value = registerConfigModel.RegistrationEmails ?? "" }
                }
            );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;
            SetTemporaryFeedbackMessage("Register configuration saved successfully.", FeedbackMessageType.Success);
            return Redirect("RegisterPagesConfig");
        }
        #endregion


        #region Site Config
        public async Task<ActionResult> SiteConfig()
        {
            return View((await _configService.ListSiteConfigsAsync("site.display"))
                .Select(x => new SiteConfigModel
                {
                    Key = x.Key,
                    Value = x.Value,
                    Name = x.Name,
                    Description = x.Description,
                    ReadOnly = x.ReadOnly
                }));
        }

        [HttpPost]
        public async Task<JsonResult> UpdateSiteConfig(IEnumerable<SiteConfigModel> values)
        {
            // Update Database Config
            await _configService.UpdateSiteConfigsAsync(
                values
                    .OrderBy(x => x.Key)
                    .Select(x => new Config
                    {
                        Key = x.Key,
                        Value = x.Value ?? "", // Store nulls as empty strings
                    })
                );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;

            return Json(new
            {
                success = true,
                redirect = "UpdateSiteConfigSuccess"
            });
        }

        public ActionResult UpdateSiteConfigSuccess()
        {
            SetTemporaryFeedbackMessage("Site configuration saved successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SiteConfig");
        }
        #endregion

        #region Sample Resource Config
        public async Task<ActionResult> SampleResourceConfig()
        {
            return View((await _configService.ListSiteConfigsAsync("site.sampleresource"))
                .Select(x => new SiteConfigModel
                {
                    Key = x.Key,
                    Value = x.Value,
                    Name = x.Name,
                    Description = x.Description,
                    ReadOnly = x.ReadOnly
                }));
        }

        public ActionResult SampleResourceConfigSuccess()
        {
            SetTemporaryFeedbackMessage("Configuration saved successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SampleResourceConfig");
        }

        #endregion
        
        //Method for updating specific Reference Terms Names via Config
        public async Task<JsonResult> UpdateReferenceTermName(string newReferenceTermKey, string newReferenceTermName)
        {

            List<Config> values = new List<Config>();

            values.Add(new Config
            {
                Key = newReferenceTermKey,
                Value = newReferenceTermName ?? ""
            });


            // Update Database Config
            await _configService.UpdateSiteConfigsAsync(values);

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;
            SetTemporaryFeedbackMessage("The Reference Term has been overriden successfully.", FeedbackMessageType.Success);
            return Json(new
            {
                success = true,
            });
        }

        #endregion

        #region Block/Allow List
        public async Task<ActionResult> BlockAllowList()
        {
            var rules = (await _registrationDomainService.ListRules())
            .Select(x => new RegistrationDomainRuleModel
            {
                Id = x.Id,
                DateModified = x.DateModified,
                RuleType = x.RuleType,
                Source = x.Source,
                Value = x.Value

            })
            .ToList();

            return View(new BlockAllowListModel
            {
                RegistrationDomainRules = rules
            });
        }

        #endregion

        private JsonResult JsonModelInvalidResponse(ModelStateDictionary state)
        {
            return Json(new
            {
                success = false,
                errors = state.Values
                    .Where(x => x.Errors.Count > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

    }
}
