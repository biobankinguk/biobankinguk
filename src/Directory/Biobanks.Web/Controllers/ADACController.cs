using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Directory.Entity.Data;
using Directory.Identity.Contracts;
using Directory.Identity.Data.Entities;
using Directory.Search.Legacy;
using Directory.Identity.Constants;
using Directory.Services.Contracts;
using Biobanks.Web.Extensions;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Biobanks.Web.Utilities;
using Biobanks.Web.Filters;
using Directory.Data.Migrations;
using System.Linq.Expressions;
using Biobanks.Web.Models.Home;
using Directory.Data.Constants;
using Biobanks.Web.Models.Search;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "ADAC")]
    public class ADACController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IEmailService _emailService;

        private readonly IBiobankIndexService _indexService;

        private readonly ISearchProvider _searchProvider;

        private readonly IMapper _mapper;
        private readonly ITokenLoggingService _tokenLog;

        public ADACController(
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IAnalyticsReportGenerator analyticsReportGenerator,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IEmailService emailService,
            IBiobankIndexService indexService,
            ISearchProvider searchProvider,
            IMapper mapper,
            ITokenLoggingService tokenLog)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _analyticsReportGenerator = analyticsReportGenerator;
            _userManager = userManager;
            _emailService = emailService;
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
                    new {
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

            return RedirectToAction("Biobanks");
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

            return RedirectToAction("Biobanks");
        }

        #endregion

        #region Funders

        public async Task<ActionResult> Funders()
        {
            return View(
                (await _biobankReadService.ListFundersAsync(string.Empty))
                    .Select(x =>

                        Task.Run(async () => new FunderModel
                        {
                            FunderId = x.FunderId,
                            Name = x.Name
                        }).Result)

                    .ToList()
                );
        }

        [HttpGet]
        public async Task<ActionResult> DeleteFunder(int id)
        {
            return View(await _biobankReadService.GetFunderByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteFunder(FunderModel model)
        {
            try
            {
                await _biobankWriteService.DeleteFunderByIdAsync(model.FunderId);

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
            if (await _biobankReadService.GetFunderbyName(model.Name) != null)
            {
                ModelState.AddModelError("Name", "That funder name is already in use. Funder names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateFunderAsync(new Funder
            {
                FunderId = model.FunderId,
                Name = model.Name
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
            if (await _biobankReadService.GetFunderbyName(model.Name) != null)
            {
                ModelState.AddModelError("Name", "That funder name is already in use. Funder names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddFunderAsync(new Funder
            {
                Name = model.Name,
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
            if (!(await _biobankReadService.GetSiteConfigStatus(ConfigKey.DisplayAnalytics)))
                return RedirectToAction("LockedRef");

            //set default options
            if (year == 0)
                year = DateTime.Today.Year;
            if (endQuarter == 0)
                endQuarter = ((DateTime.Today.Month + 2) / 3);
            if (reportPeriod == 0)
                reportPeriod = 10;

            var model = _mapper.Map<DirectoryAnalyticReport>(await _analyticsReportGenerator.GetDirectoryReport(year, endQuarter, reportPeriod));
            return View(model);
        }
        #endregion

        #region Reference Datasets

        #region RefData: Access Conditions
        public async Task<ActionResult> AccessConditions()
        {
            var models = (await _biobankReadService.ListAccessConditionsAsync())
                .Select(x =>
                    Task.Run(async () => new ReadAccessConditionsModel
                    {
                        Id = x.AccessConditionId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        AccessConditionCount = await _biobankReadService.GetAccessConditionsCount(x.AccessConditionId),
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

        [HttpPost]
        public async Task<JsonResult> AddAccessConditionAjax(AccessConditionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidAccessConditionDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Access condition descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var access = new AccessCondition
            {
                AccessConditionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddAccessConditionAsync(access);
            await _biobankWriteService.UpdateAccessConditionAsync(access, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddAccessConditionSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditAccessConditionAjax(AccessConditionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidAccessConditionDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another access condition. Access condition descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAccessConditionInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This access condition is currently in use and cannot be edited." }
                });
            }

            var access = new AccessCondition
            {
                AccessConditionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateAccessConditionAsync(access);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditAccessConditionSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteAccessCondition(AccessConditionModel model)
        {
            if (await _biobankReadService.IsAccessConditionInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The access condition \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("AccessConditions");
            }

            await _biobankWriteService.DeleteAccessConditionAsync(new AccessCondition
            {
                AccessConditionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The access condition \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AccessConditions");
        }

        public ActionResult AddAccessConditionSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The access condition \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AccessConditions");
        }

        public ActionResult EditAccessConditionSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The access condition \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AccessConditions");
        }
        #endregion

        #region RefData: Age Ranges
        public async Task<ActionResult> AgeRanges()
        {
            var models = (await _biobankReadService.ListAgeRangesAsync())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.AgeRangeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetAgeRangeUsageCount(x.AgeRangeId)
                    })
                    .Result
                )
                .ToList();

            return View(new AgeRangesModel
            {
                AgeRanges = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddAgeRangeAjax(AgeRangeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Age Range
            var range = new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddAgeRangeAsync(range);
            await _biobankWriteService.UpdateAgeRangeAsync(range, true); // Ensure sortOrder is correct

            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddAgeRangeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditAgeRangeAjax(AgeRangeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateAgeRangeAsync(new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditAgeRangeSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteAgeRange(AgeRangeModel model)
        {
            if (await _biobankReadService.IsAgeRangeInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The age range \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("AgeRanges");
            }

            await _biobankWriteService.DeleteAgeRangeAsync(new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            SetTemporaryFeedbackMessage($"The age range  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AgeRanges");
        }

        public ActionResult AddAgeRangeSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The age range \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AgeRanges");
        }

        public ActionResult EditAgeRangeSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The age range \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AgeRanges");
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
                    Id = x.AssociatedDataProcurementTimeframeId,
                    Description = x.Description,
                    DisplayName = x.DisplayValue,
                    CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(x.AssociatedDataProcurementTimeframeId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteAssociatedDataProcurementTimeFrame(Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            //Validate min amount of time frames
            var timeFrames = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();
            if (timeFrames.Count() <= 2)
            {
                SetTemporaryFeedbackMessage($"A minimum amount of 2 time frames are allowed.", FeedbackMessageType.Warning);
                return RedirectToAction("AssociatedDataProcurementTimeFrame");
            }

            if (await _biobankReadService.IsAssociatedDataProcurementTimeFrameInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The associated data procurement time frame \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("AssociatedDataProcurementTimeFrame");
            }

            await _biobankWriteService.DeleteAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The associated data procurement time frame \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataProcurementTimeFrame");

        }

        [HttpPost]
        public async Task<JsonResult> EditAssociatedDataProcurementTimeFrameAjax(Models.Shared.AssociatedDataProcurementTimeFrameModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidAssociatedDataProcurementTimeFrameDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That Associated Data Procurement Time Frame already exists!");
            }

            if (model.DisplayName == null)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field is required.");
            }

            if (model.DisplayName.Length > 10)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field allows a maximum of 10 characters.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsAssociatedDataProcurementTimeFrameInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description,
                DisplayValue = model.DisplayName,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditAssociatedDataProcurementTimeFrameSuccess?name={model.Description}"
            });
        }

        public ActionResult EditAssociatedDataProcurementTimeFrameSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data procurement time frame \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataProcurementTimeFrame");
        }

        [HttpPost]
        public async Task<JsonResult> AddAssociatedDataProcurementTimeFrameAjax(Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            var timeFrames = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();
            if (timeFrames.Count() >= 5)
            {
                SetTemporaryFeedbackMessage($"A maximum amount of 5 time frames are allowed.", FeedbackMessageType.Warning);
                return Json(new
                {
                    success = true,
                    redirect = $"AssociatedDataProcurementTimeFrame"
                });
            }

            if (await _biobankReadService.ValidAssociatedDataProcurementTimeFrameDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That description is already in use. Associated Data Procurement Time Frame descriptions must be unique.");
            }

            if (model.DisplayName == null)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field is required.");
            }

            if (model.DisplayName.Length > 10)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field allows a maximum of 10 characters.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var procurement = new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            };

            await _biobankWriteService.AddAssociatedDataProcurementTimeFrameAsync(procurement);
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(procurement, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddAssociatedDataProcurementTimeFrameSuccess?name={model.Description}"
            });
        }
        public ActionResult AddAssociatedDataProcurementTimeFrameSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data procurement time frame \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataProcurementTimeFrame");
        }
        #endregion

        #region RefData: AnnualStatistics
        public async Task<ActionResult> AnnualStatistics()
        {
            var groups = (await _biobankReadService.ListAnnualStatisticGroupsAsync())
                .Select(x => new AnnualStatisticGroupModel
                {
                    AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                    Name = x.Name,
                })
                .ToList();

            var models = (await _biobankReadService.ListAnnualStatisticsAsync())
                .Select(x =>
                    Task.Run(async () => new AnnualStatisticModel
                    {
                        Id = x.AnnualStatisticId,
                        Name = x.Name,
                        UsageCount = await _biobankReadService.GetAnnualStatisticUsageCount(x.AnnualStatisticId),
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

        [HttpPost]
        public async Task<JsonResult> AddAnnualStatisticAjax(AnnualStatisticModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAnnualStatisticAsync(model.Name, model.AnnualStatisticGroupId))
            {
                ModelState.AddModelError("AnnualStatistics", "That name is already in use. Annual statistics names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var annualStatistic = new AnnualStatistic
            {
                AnnualStatisticId = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.AddAnnualStatisticAsync(annualStatistic);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"AddAnnualStatisticSuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditAnnualStatisticAjax(AnnualStatisticModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAnnualStatisticAsync(model.Name, model.AnnualStatisticGroupId))
            {
                ModelState.AddModelError("AnnualStatistics", "That annual statistic already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAnnualStatisticInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This annual statistic is currently in use and cannot be edited." }
                });
            }

            var annualStatistics = new AnnualStatistic
            {
                AnnualStatisticId = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.UpdateAnnualStatisticAsync(annualStatistics);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"EditAnnualStatisticSuccess?name={model.Name}"
            });
        }

        public async Task<ActionResult> DeleteAnnualStatistic(AnnualStatisticModel model)
        {
            if (await _biobankReadService.IsAnnualStatisticInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The annual statistic \"{model.Name}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("AnnualStatistics");
            }

            var annualStatistic = new AnnualStatistic
            {
                AnnualStatisticId = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.DeleteAnnualStatisticAsync(annualStatistic);

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The annual statistics type \"{model.Name}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AnnualStatistics");
        }

        public ActionResult AddAnnualStatisticSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The annual statistic \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AnnualStatistics");
        }

        public ActionResult EditAnnualStatisticSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The annual statistic \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("AnnualStatistics");
        }

        #endregion

        #region RefData: Material Types
        public async Task<ActionResult> MaterialTypes()
        {
            return View(new MaterialTypesModel
            {
                MaterialTypes = (await _biobankReadService.ListMaterialTypesAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadMaterialTypeModel
                    {
                        Id = x.MaterialTypeId,
                        Description = x.Description,
                        MaterialDetailCount = await _biobankReadService.GetMaterialTypeMaterialDetailCount(x.MaterialTypeId),
                        SortOrder = x.SortOrder
                    }).Result)

                    .ToList()
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddMaterialTypeAjax(MaterialTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddMaterialTypeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditMaterialTypeAjax(MaterialTypeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material types must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = false;

            // Update Preservation Type
            await _biobankWriteService.UpdateMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditMaterialTypeSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteMaterialType(MaterialTypeModel model)
        {
            if (await _biobankReadService.IsMaterialTypeInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The material type \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("MaterialTypes");
            }

            await _biobankWriteService.DeleteMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The material type \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("MaterialTypes");
        }

        public ActionResult EditMaterialTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The material type \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("MaterialTypes");
        }

        public ActionResult AddMaterialTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The material type \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("MaterialTypes");
        }
        #endregion

        #region RefData: Disease Status
        public async Task<ActionResult> DiseaseStatuses()
        {
            return View(new DiagnosesModel
            {
                Diagnoses = (await _biobankReadService.ListDiagnosesAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadDiagnosisModel
                    {
                        Id = x.DiagnosisId,
                        SnomedIdentifier = x.SnomedIdentifier,
                        Description = x.Description,
                        CollectionCapabilityCount = await _biobankReadService.GetDiagnosisCollectionCapabilityCount(x.DiagnosisId),
                        OtherTerms = x.OtherTerms
                    }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteDiseaseStatus(DiagnosisModel model)
        {
            if (await _biobankReadService.IsDiagnosisInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The disease status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("DiseaseStatuses");
            }

            await _biobankWriteService.DeleteDiagnosisAsync(new Diagnosis
            {
                DiagnosisId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The disease status \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("DiseaseStatuses");
        }

        [HttpPost]
        public async Task<JsonResult> EditDiseaseStatusAjax(DiagnosisModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another disease status. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsDiagnosisInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This disease status is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateDiagnosisAsync(new Diagnosis
            {
                DiagnosisId = model.Id,
                SnomedIdentifier = model.SnomedIdentifier,
                Description = model.Description,
                OtherTerms = model.OtherTerms
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult EditDiseaseStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The disease status \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("DiseaseStatuses");
        }

        [HttpPost]
        public async Task<JsonResult> AddDiseaseStatusAjax(DiagnosisModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddDiagnosisAsync(new Diagnosis
            {
                Description = model.Description,
                SnomedIdentifier = model.SnomedIdentifier,
                OtherTerms = model.OtherTerms
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult AddDiseaseStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The disease status \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("DiseaseStatuses");
        }
        #endregion

        #region RefData: Collection Percentages
        public async Task<ActionResult> CollectionPercentages()
        {
            var models = (await _biobankReadService.ListCollectionPercentagesAsync())
                .Select(x =>
                    Task.Run(async () => new CollectionPercentageModel()
                    {
                        Id = x.CollectionPercentageId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetCollectionPercentageUsageCount(x.CollectionPercentageId)
                    })
                    .Result
                )
                .ToList();
            if (await _biobankReadService.GetSiteConfigStatus("site.display.preservation.percent") == true)
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

        [HttpPost]
        public async Task<JsonResult> AddCollectionPercentageAjax(CollectionPercentageModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionPercentageAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That description is already in use. Collection percentage descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var percentage = new CollectionPercentage
            {
                CollectionPercentageId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound,
            };

            await _biobankWriteService.AddCollectionPercentageAsync(percentage);
            await _biobankWriteService.UpdateCollectionPercentageAsync(percentage, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddCollectionPercentageSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditCollectionPercentageAjax(CollectionPercentageModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionPercentageAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That collection percentage already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionPercentageAsync(new CollectionPercentage
            {
                CollectionPercentageId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditCollectionPercentageSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteCollectionPercentage(CollectionPercentageModel model)
        {
            if (await _biobankReadService.IsCollectionPercentageInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The collection percentage \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("CollectionPercentage");
            }

            await _biobankWriteService.DeleteCollectionPercentageAsync(new CollectionPercentage
            {
                CollectionPercentageId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            SetTemporaryFeedbackMessage($"The collection percentage  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("CollectionPercentages");
        }

        public ActionResult AddCollectionPercentageSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The collection percentage \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("CollectionPercentages");
        }

        public ActionResult EditCollectionPercentageSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The collection percentage \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("CollectionPercentages");
        }
        #endregion

        #region RefData: Collection Points
        public async Task<ActionResult> CollectionPoints()
        {
            var models = (await _biobankReadService.ListCollectionPointsAsync())
                .Select(x =>
                    Task.Run(async () => new CollectionPointModel()
                    {
                        Id = x.CollectionPointId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    })
                    .Result
                )
                .ToList();

            return View(new CollectionPointsModel()
            {
                CollectionPoints = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddCollectionPointAjax(CollectionPointModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionPointDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPoints", "That description is already in use. collection point descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var points = new CollectionPoint
            {
                CollectionPointId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddCollectionPointAsync(points);
            await _biobankWriteService.UpdateCollectionPointAsync(points, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddCollectionPointSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditCollectionPointAjax(CollectionPointModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionPointDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPoints", "That collection point already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditCollectionPointSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteCollectionPoint(CollectionPointModel model)
        {
            if (await _biobankReadService.IsCollectionPointInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The collection point \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("CollectionPoints");
            }

            await _biobankWriteService.DeleteCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            SetTemporaryFeedbackMessage($"The collection point  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("CollectionPoints");
        }

        public ActionResult AddCollectionPointSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The collection point \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("CollectionPoints");
        }

        public ActionResult EditCollectionPointSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The collection point \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("CollectionPoints");
        }
        #endregion

        #region RefData: Donor Counts
        public async Task<ActionResult> DonorCounts()
        {
            var models = (await _biobankReadService.ListDonorCountsAsync(true))
                .Select(x =>
                    Task.Run(async () => new DonorCountModel()
                    {
                        Id = x.DonorCountId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetDonorCountUsageCount(x.DonorCountId)
                    })
                    .Result
                )
                .ToList();

            return View(new DonorCountsModel()
            {
                DonorCounts = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddDonorCountAjax(DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (await _biobankReadService.ValidDonorCountAsync(model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var donor = new DonorCount
            {
                DonorCountId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound

            };

            await _biobankWriteService.AddDonorCountAsync(donor);
            await _biobankWriteService.UpdateDonorCountAsync(donor, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddDonorCountSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditDonorCountAjax(DonorCountModel model, bool sortOnly = false)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (!sortOnly && await _biobankReadService.ValidDonorCountAsync(model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That {currentReferenceName.Value} already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateDonorCountAsync(new DonorCount
            {
                DonorCountId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditDonorCountSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        public async Task<ActionResult> DeleteDonorCount(DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

            if (await _biobankReadService.IsDonorCountInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The {currentReferenceName.Value} \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("DonorCount");
            }

            await _biobankWriteService.DeleteDonorCountAsync(new DonorCount
            {
                DonorCountId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            SetTemporaryFeedbackMessage($"The {currentReferenceName.Value}  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("DonorCounts");
        }

        public ActionResult AddDonorCountSuccess(string name, string referencename)
        {
            SetTemporaryFeedbackMessage($"The {referencename} \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("DonorCounts");
        }

        public ActionResult EditDonorCountSuccess(string name, string referencename)
        {
            SetTemporaryFeedbackMessage($"The {referencename} \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("DonorCounts");
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
                    Id = x.CollectionTypeId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetCollectionTypeCollectionCount(x.CollectionTypeId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteCollectionType(Models.Shared.CollectionTypeModel model)
        {
            if (await _biobankReadService.IsCollectionTypeInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The collection type \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("CollectionType");
            }

            await _biobankWriteService.DeleteCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The collection type \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionType");

        }

        [HttpPost]
        public async Task<JsonResult> EditCollectionTypeAjax(Models.Shared.CollectionTypeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionType", "That collection type already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsCollectionTypeInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult EditCollectionTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The collection type \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionType");
        }

        [HttpPost]
        public async Task<JsonResult> AddCollectionTypeAjax(Models.Shared.CollectionTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCollectionTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection types descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddCollectionTypeAsync(new CollectionType
            {
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult AddCollectionTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The consent restriction \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionType");
        }
        #endregion

        #region RefData: Preservation Type
        [HttpPost]
        public async Task<JsonResult> AddPreservationTypeAjax(PreservationTypeModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // Validate model
            if (await _biobankReadService.ValidPreservationTypeAsync(model.Description))
            {
                ModelState.AddModelError("PreservationType", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Preservation Type
            var type = new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddPreservationTypeAsync(type);
            await _biobankWriteService.UpdatePreservationTypeAsync(type, true); // Ensure sortOrder is correct

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddPreservationTypeSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        public async Task<ActionResult> PreservationTypes()
        {
            var models = (await _biobankReadService.ListPreservationTypesAsync())
                .Select(x =>
                    new PreservationTypeModel()
                    {
                        Id = x.PreservationTypeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    }
                )
                .ToList();

            // Fetch Sample Set Count
            foreach (var model in models)
            {
                model.SampleSetsCount = await _biobankReadService.GetPreservationTypeUsageCount(model.Id);
            }

            return View(new PreservationTypesModel
            {
                PreservationTypes = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditPreservationTypeAjax(PreservationTypeModel model, bool sortOnly = false)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // Validate model
            if (!sortOnly && await _biobankReadService.ValidPreservationTypeAsync(model.Description))
            {
                ModelState.AddModelError("PreservationType", $"That {currentReferenceName.Value} already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdatePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));



            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditPreservationTypeSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        public async Task<ActionResult> DeletePreservationType(PreservationTypeModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);
            if (await _biobankReadService.IsPreservationTypeInUse(model.Id))
            {

                SetTemporaryFeedbackMessage($"The {currentReferenceName.Value} \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("PreservationTypes");
            }

            await _biobankWriteService.DeletePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            SetTemporaryFeedbackMessage($"The {currentReferenceName.Value}  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("PreservationTypes");
        }

        public ActionResult AddPreservationTypeSuccess(string name, string referencename)
        {
            SetTemporaryFeedbackMessage($"The {referencename} \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("PreservationTypes");
        }

        public ActionResult EditPreservationTypeSuccess(string name, string referencename)
        {
            SetTemporaryFeedbackMessage($"The {referencename} \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("PreservationTypes");
        }
        #endregion

        #region RefData: Assocaited Data Types
        public async Task<ActionResult> AssociatedDataTypes()
        {
            var groups = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync())
                .Select(x => new AssociatedDataTypeGroupModel
                {
                    AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                    Name = x.Description,
                })
                .ToList();
            var model = (await _biobankReadService.ListAssociatedDataTypesAsync()).Select(x =>

            Task.Run(async () => new AssociatedDataTypeModel
            {
                Id = x.AssociatedDataTypeId,
                Name = x.Description,
                Message = x.Message,
                CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataTypeCollectionCapabilityCount(x.AssociatedDataTypeId),
                AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                AssociatedDataTypeGroupName = groups.Where(y => y.AssociatedDataTypeGroupId == x.AssociatedDataTypeGroupId).FirstOrDefault()?.Name,

            }).Result)

               .ToList();


            return View(new AssociatedDataTypesModel
            {
                AssociatedDataTypes = model,
                AssociatedDataTypeGroups = groups
            });
        }

        public async Task<ActionResult> DeleteAssociatedDataType(AssociatedDataTypeModel model)
        {
            if (await _biobankReadService.IsAssociatedDataTypeInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The associated data type \"{model.Name}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("AssociatedDataTypes");
            }

            await _biobankWriteService.DeleteAssociatedDataTypeAsync(new AssociatedDataType
            {
                AssociatedDataTypeId = model.Id,
                Description = model.Name
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The associated data type \"{model.Name}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypes");
        }

        [HttpPost]
        public async Task<JsonResult> EditAssociatedDataTypeAjax(AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAssociatedDataTypeDescriptionAsync(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That associated data type already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAssociatedDataTypeInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This associated data type is currently in use and cannot be edited." }
                });
            }

            var associatedDataTypes = new AssociatedDataType
            {
                AssociatedDataTypeId = model.Id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name,
                Message = model.Message

            };

            await _biobankWriteService.UpdateAssociatedDataTypeAsync(associatedDataTypes);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"EditAssociatedDataTypeSuccess?name={model.Name}"
            });
        }

        public ActionResult EditAssociatedDataTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data type \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypes");
        }

        [HttpPost]
        public async Task<JsonResult> AddAssociatedDataTypeAjax(AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAssociatedDataTypeDescriptionAsync(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That name is already in use. Associated Data Type names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var associatedDataType = new AssociatedDataType
            {
                AssociatedDataTypeId = model.Id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name,
                Message = model.Message
            };

            await _biobankWriteService.AddAssociatedDataTypeAsync(associatedDataType);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"AddAssociatedDataTypeSuccess?name={model.Name}"
            });
        }

        public ActionResult AddAssociatedDataTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data type \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypes");
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
                        AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                        Name = x.Description,
                        AssociatedDataTypeGroupCount = await _biobankReadService.GetAssociatedDataTypeGroupCount(x.AssociatedDataTypeGroupId)
                    }).Result)

                    .ToList()
            });
        }
        public async Task<ActionResult> DeleteAssociatedDataTypeGroup(AssociatedDataTypeGroupModel model)
        {
            if (await _biobankReadService.IsAssociatedDataTypeGroupInUse(model.AssociatedDataTypeGroupId))
            {
                SetTemporaryFeedbackMessage(
                    $"The associated data type group \"{model.Name}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("AssociatedDataTypeGroups");
            }

            await _biobankWriteService.DeleteAssociatedDataTypeGroupAsync(new Directory.Entity.Data.AssociatedDataTypeGroup
            {
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The associated data type group \"{model.Name}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypeGroups");
        }

        [HttpPost]
        public async Task<JsonResult> AddAssociatedDataTypeGroupAjax(AssociatedDataTypeGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAssociatedDataTypeGroupNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddAssociatedDataTypeGroupAsync(new Directory.Entity.Data.AssociatedDataTypeGroup
            {
                Description = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult AddAssociatedDataTypeGroupSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data type group \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypeGroups");
        }

        [HttpPost]
        public async Task<JsonResult> EditAssociatedDataTypeGroupAjax(AssociatedDataTypeGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAssociatedDataTypeGroupNameAsync(model.AssociatedDataTypeGroupId, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another asscoiated data type group. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAssociatedDataTypeGroupInUse(model.AssociatedDataTypeGroupId))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This associated data type group is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateAssociatedDataTypeGroupAsync(new Directory.Entity.Data.AssociatedDataTypeGroup
            {
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult EditAssociatedDataTypeGroupSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data type group \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypeGroups");
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
                    Id = x.ConsentRestrictionId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetConsentRestrictionCollectionCount(x.ConsentRestrictionId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteConsentRestriction(Models.Shared.ConsentRestrictionModel model)
        {
            if (await _biobankReadService.IsConsentRestrictionInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The consent restriction \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("ConsentRestriction");
            }

            await _biobankWriteService.DeleteConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The consent restriction \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ConsentRestriction");

        }

        [HttpPost]
        public async Task<JsonResult> EditConsentRestrictionAjax(Models.Shared.ConsentRestrictionModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidConsentRestrictionDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("ConsentRestriction", "That consent restriction already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsConsentRestrictionInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult EditConsentRestrictionSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The consent restriction \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ConsentRestriction");
        }

        [HttpPost]
        public async Task<JsonResult> AddConsentRestrictionAjax(Models.Shared.ConsentRestrictionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Consent restriction descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddConsentRestrictionAsync(new ConsentRestriction
            {
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult AddConsentRestrictionSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The consent restriction \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ConsentRestriction");
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
                    Id = x.CollectionStatusId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetCollectionStatusCollectionCount(x.CollectionStatusId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteCollectionStatus(Models.Shared.CollectionStatusModel model)
        {
            if (await _biobankReadService.IsCollectionStatusInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The collection status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("CollectionStatus");
            }

            await _biobankWriteService.DeleteCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The collection status \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionStatus");

        }

        [HttpPost]
        public async Task<JsonResult> EditCollectionStatusAjax(Models.Shared.CollectionStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionStatus", "That collection status already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsCollectionStatusInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult EditCollectionStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The collection status \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionStatus");
        }

        [HttpPost]
        public async Task<JsonResult> AddCollectionStatusAjax(Models.Shared.CollectionStatusModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCollectionStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddCollectionStatusAsync(new CollectionStatus
            {
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult AddCollectionStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The collection status \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionStatus");
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
                        AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                        Name = x.Name,
                        AnnualStatisticGroupCount = await _biobankReadService.GetAnnualStatisticAnnualStatisticGroupCount(x.AnnualStatisticGroupId)
                    }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteAnnualStatisticGroup(AnnualStatisticGroupModel model)
        {
            if (await _biobankReadService.IsAnnualStatisticGroupInUse(model.AnnualStatisticGroupId))
            {
                SetTemporaryFeedbackMessage(
                    $"The annual statistic group \"{model.Name}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("AnnualStatisticGroups");
            }

            await _biobankWriteService.DeleteAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The annual statistic group \"{model.Name}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AnnualStatisticGroups");
        }

        [HttpPost]
        public async Task<JsonResult> EditAnnualStatisticGroupAjax(AnnualStatisticGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAnnualStatisticGroupNameAsync(model.AnnualStatisticGroupId, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another annual statistic group. Annual Statistic Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAnnualStatisticGroupInUse(model.AnnualStatisticGroupId))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This annual statistic group is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult EditAnnualStatisticGroupSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The annual statistic group \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AnnualStatisticGroups");
        }

        [HttpPost]
        public async Task<JsonResult> AddAnnualStatisticGroupAjax(AnnualStatisticGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAnnualStatisticGroupNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Annual Statistic Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult AddAnnualStatisticGroupSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The annual statistic group \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AnnualStatisticGroups");
        }

        #endregion

        #region RefData: Sample Collection Mode
        public async Task<ActionResult> SampleCollectionModes()
        {
            var models = (await _biobankReadService.ListSampleCollectionModeAsync())
                .Select(x =>
                    Task.Run(async () => new SampleCollectionModeModel
                    {
                        Id = x.SampleCollectionModeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetSampleCollectionModeUsageCount(x.SampleCollectionModeId)
                    })
                    .Result
                )
                .ToList();

            return View(new SampleCollectionModesModel()
            {
                SampleCollectionModes = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddSampleCollectionModeAjax(SampleCollectionModeModel model)
        {
            //// Validate model
            if (await _biobankReadService.ValidSampleCollectionModeAsync(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That description is already in use. Sample collection modes must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddSampleCollectionModeAsync(mode);
            await _biobankWriteService.UpdateSampleCollectionModeAsync(mode, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddSampleCollectionModeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditSampleCollectionModeAjax(SampleCollectionModeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidSampleCollectionModeAsync(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That sample collection modes already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateSampleCollectionModeAsync(mode);

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditSampleCollectionModeSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteSampleCollectionMode(SampleCollectionModeModel model)
        {
            if (await _biobankReadService.IsSampleCollectionModeInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The sample collection mode \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("SampleCollectionModes");
            }

            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.DeleteSampleCollectionModeAsync(mode);

            // Success
            SetTemporaryFeedbackMessage($"The sample colelction mode  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SampleCollectionModes");
        }

        public ActionResult AddSampleCollectionModeSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The sameple collection mode \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SampleCollectionModes");
        }

        public ActionResult EditSampleCollectionModeSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The sample collection mode \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SampleCollectionModes");
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
                        Id = x.SexId,
                        Description = x.Description,
                        SexCount = await _biobankReadService.GetSexCount(x.SexId),
                        SortOrder = x.SortOrder
                    }).Result)

                    .ToList()
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddSexAjax(SexModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSexDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Sex descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddSexAsync(new Sex
            {
                SexId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddSexSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditSexAjax(SexModel model, bool sortOnly = false)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSexDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another material type. Sex descriptions must be unique.");
            }

            if (await _biobankReadService.IsSexInUse(model.Id))
            {
                ModelState.AddModelError("Description", "This sex is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateSexAsync(new Sex
            {
                SexId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditSexSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteSex(SexModel model)
        {
            if (await _biobankReadService.IsSexInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The sex \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Sexes");
            }

            await _biobankWriteService.DeleteSexAsync(new Sex
            {
                SexId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The sex \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Sexes");
        }

        public ActionResult AddSexSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The sex \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("Sexes");
        }

        public ActionResult EditSexSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The sex \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("Sexes");
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
                        Id = x.CountryId,
                        Name = x.Name,
                        CountyOrganisationCount = await _biobankReadService.GetCountryCountyOrganisationCount(x.CountryId)
                    }).Result)

                    .ToList()
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddCountryAjax(Models.Shared.CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Country names must be unique.");
            }

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

            await _biobankWriteService.AddCountryAsync(new Country
            {
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult AddCountrySuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The country \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Country");
        }

        [HttpPost]
        public async Task<JsonResult> EditCountryAjax(Models.Shared.CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(model.Id, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another country. Country names must be unique.");
            }

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

            if (await _biobankReadService.IsCountryInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This country is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateCountryAsync(new Country
            {
                CountryId = model.Id,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult EditCountrySuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The country \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Country");
        }

        public async Task<ActionResult> DeleteCountry(Models.Shared.CountryModel model)
        {
            if (await _biobankReadService.IsCountryInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The country \"{model.Name}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Country");
            }

            await _biobankWriteService.DeleteCountryAsync(new Country
            {
                CountryId = model.Id,
                Name = model.Name
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The country \"{model.Name}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Country");
        }
        #endregion

        #region RefData: County
        public async Task<ActionResult> County()
        {
            if (await _biobankReadService.GetSiteConfigStatus("site.display.counties") == true)
            {
                var countries = await _biobankReadService.ListCountriesAsync();

                return View(
                    new CountiesModel
                    {
                        Counties = countries.ToDictionary(
                            x => x.Name,
                            x => x.Counties.Select(county =>
                                Task.Run(async () =>
                                    new CountyModel
                                    {
                                        Id = county.CountyId,
                                        CountryId = x.CountryId,
                                        Name = county.Name,
                                        CountyUsageCount = await _biobankReadService.GetCountyUsageCount(county.CountyId)
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

        [HttpPost]
        public async Task<JsonResult> AddCountyAjax(CountyModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCountyAsync(model.Name))
            {
                ModelState.AddModelError("County", "That name is already in use. County names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var county = new County
            {
                CountyId = model.Id,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _biobankWriteService.AddCountyAsync(county);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"AddCountySuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditCountyAjax(CountyModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCountyAsync(model.Name))
            {
                ModelState.AddModelError("County", "That county already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsCountyInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This county is currently in use and cannot be edited." }
                });
            }

            var county = new County
            {
                CountyId = model.Id,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _biobankWriteService.UpdateCountyAsync(county);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"EditCountySuccess?name={model.Name}"
            });
        }

        public async Task<ActionResult> DeleteCounty(CountyModel model)
        {
            if (await _biobankReadService.IsCountyInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The county \"{model.Name}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("Country");
            }

            var county = new County
            {
                CountyId = model.Id,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _biobankWriteService.DeleteCountyAsync(county);

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The county type \"{model.Name}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("County");
        }

        public ActionResult AddCountySuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The county \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("County");
        }

        public ActionResult EditCountySuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The county \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("County");
        }

        #endregion

        #region RefData: Sop Status
        public async Task<ActionResult> SopStatus()
        {
            var models = (await _biobankReadService.ListSopStatusesAsync())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.SopStatusId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    })
                    .Result
                )
                .ToList();

            return View(new SopStatusesModel()
            {
                SopStatuses = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddSopStatusAjax(SopStatusModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidSopStatusAsync(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That description is already in use. Sop status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var status = new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddSopStatusAsync(status);
            await _biobankWriteService.UpdateSopStatusAsync(status, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddSopStatusSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditSopStatusAjax(SopStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidSopStatusAsync(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That sop status already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateSopStatusAsync(new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditSopStatusSuccess?name={model.Description}"
            });
        }

        public async Task<ActionResult> DeleteSopStatus(SopStatusModel model)
        {
            if (await _biobankReadService.IsSopStatusInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The sop status \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("SopStatus");
            }

            await _biobankWriteService.DeleteSopStatusAsync(new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            SetTemporaryFeedbackMessage($"The sop status  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SopStatus");
        }

        public ActionResult AddSopStatusSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The sop status \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SopStatus");
        }

        public ActionResult EditSopStatusSuccess(string name)
        {
            SetTemporaryFeedbackMessage($"The sop status \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("SopStatus");
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
                    Id = x.RegistrationReasonId,
                    Description = x.Description,
                    OrganisationCount = await _biobankReadService.GetRegistrationReasonOrganisationCount(x.RegistrationReasonId),
                }).Result)

                    .ToList()
            });
        }

        public async Task<ActionResult> DeleteRegistrationReason(Models.Shared.RegistrationReasonModel model)
        {
            if (await _biobankReadService.IsRegistrationReasonInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The registration reason \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("RegistrationReason");
            }

            await _biobankWriteService.DeleteRegistrationReasonAsync(new RegistrationReason
            {
                RegistrationReasonId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The registration reason \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("RegistrationReason");
        }

        [HttpPost]
        public async Task<JsonResult> EditRegistrationReasonAjax(Models.Shared.RegistrationReasonModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidRegistrationReasonDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another registration reason. Registration reason descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsRegistrationReasonInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This registration reason is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateRegistrationReasonAsync(new RegistrationReason
            {
                RegistrationReasonId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult EditRegistrationReasonSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The registration reason \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("RegistrationReason");
        }

        [HttpPost]
        public async Task<JsonResult> AddRegistrationReasonAjax(Models.Shared.RegistrationReasonModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidRegistrationReasonDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Registration reason descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddRegistrationReasonAsync(new RegistrationReason
            {
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult AddRegistrationReasonSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The registration reason \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("RegistrationReason");
        }
        #endregion

        #region RefData: Macroscopic Assessment
        public async Task<ActionResult> MacroscopicAssessments()
        {
            var models = (await _biobankReadService.ListMacroscopicAssessmentsAsync())
                .Select(x =>
                    Task.Run(async () => new MacroscopicAssessmentModel()
                    {
                        Id = x.MacroscopicAssessmentId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetMacroscopicAssessmentUsageCount(x.MacroscopicAssessmentId)
                    })
                    .Result
                )
                .ToList();

            return View(new MacroscopicAssessmentsModel()
            {
                MacroscopicAssessments = models
            });
        }

        [HttpPost]
        public async Task<JsonResult> AddMacroscopicAssessmentAjax(MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            // Validate model
            if (await _biobankReadService.ValidMacroscopicAssessmentAsync(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var assessment = new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddMacroscopicAssessmentAsync(assessment);
            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(assessment, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddMacroscopicAssessmentSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditMacroscopicAssessmentAjax(MacroscopicAssessmentModel model, bool sortOnly = false)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            //// Validate model
            if (!sortOnly && await _biobankReadService.ValidMacroscopicAssessmentAsync(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditMacroscopicAssessmentSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        public async Task<ActionResult> DeleteMacroscopicAssessment(MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            if (await _biobankReadService.IsMacroscopicAssessmentInUse(model.Id))
            {
                SetTemporaryFeedbackMessage($"The {currentReferenceName.Value} \"{model.Description}\" is currently in use, and cannot be deleted.", FeedbackMessageType.Danger);
                return RedirectToAction("MacroscopicAssessments");
            }

            if ((await _biobankReadService.ListMacroscopicAssessmentsAsync()).Count() <= 1)
            {
                SetTemporaryFeedbackMessage($"The {currentReferenceName.Value} \"{model.Description}\" is currently the last entry and cannot be deleted", FeedbackMessageType.Danger);
                return RedirectToAction("MacroscopicAssessments");
            }

            await _biobankWriteService.DeleteMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            SetTemporaryFeedbackMessage($"The {currentReferenceName.Value}  \"{model.Description}\" was deleted successfully.", FeedbackMessageType.Success);
            return RedirectToAction("MacroscopicAssessments");
        }

        public ActionResult AddMacroscopicAssessmentSuccess(string name, string referencename)
        {
            SetTemporaryFeedbackMessage($"The {referencename} \"{name}\" has been added successfully.", FeedbackMessageType.Success);
            return RedirectToAction("MacroscopicAssessments");
        }

        public ActionResult EditMacroscopicAssessmentSuccess(string name, string referencename)
        {
            SetTemporaryFeedbackMessage($"The {referencename} \"{name}\" has been edited successfully.", FeedbackMessageType.Success);
            return RedirectToAction("MacroscopicAssessments");
        }
        #endregion

        #region RefData: HtaStatus
        public async Task<ActionResult> HtaStatus()
        {
            if (await _biobankReadService.GetSiteConfigStatus(ConfigKey.EnableHTA) == true)
            {
                return View(new Models.ADAC.HtaStatusModel
                {
                    HtaStatuses = (await _biobankReadService.ListHtaStatusesAsync())
                        .Select(x =>

                    Task.Run(async () => new ReadHtaStatusModel
                    {
                        Id = x.HtaStatusId,
                        Description = x.Description,
                        CollectionCount = await _biobankReadService.GetHtaStatusCollectionCount(x.HtaStatusId),
                        SortOrder = x.SortOrder
                    }).Result)

                        .ToList()
                });
            }
            else
            {
                return RedirectToAction("LockedRef");
            }

        }


        public async Task<ActionResult> DeleteHtaStatus(Models.Shared.HtaStatusModel model)
        {
            if (await _biobankReadService.IsHtaStatusInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The hta status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("HtaStatus");
            }

            await _biobankWriteService.DeleteHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The hta status \"{model.Description}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("HtaStatus");

        }

        [HttpPost]
        public async Task<JsonResult> EditHtaStatusAjax(Models.Shared.HtaStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("HtaStatus", "That hta status already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsHtaStatusInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult EditHtaStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The hta status \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("HtaStatus");
        }

        [HttpPost]
        public async Task<JsonResult> AddHtaStatusAjax(Models.Shared.HtaStatusModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Hta status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddHtaStatusAsync(new HtaStatus
            {
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        public ActionResult AddHtaStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The hta status \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("HtaStatus");
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
                    Id = x.ServiceId,
                    Name = x.Name,
                    OrganisationCount = await _biobankReadService.GetServiceOfferingOrganisationCount(x.ServiceId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList()
            });
        }
        public async Task<ActionResult> DeleteServiceOffering(Models.Shared.ServiceOfferingModel model)
        {
            if (await _biobankReadService.IsServiceOfferingInUse(model.Id))
            {
                SetTemporaryFeedbackMessage(
                    $"The service offering \"{model.Name}\" is currently in use, and cannot be deleted.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("ServiceOffering");
            }

            await _biobankWriteService.DeleteServiceOfferingAsync(new ServiceOffering
            {
                ServiceId = model.Id,
                Name = model.Name
            });

            //Everything went A-OK!
            SetTemporaryFeedbackMessage($"The service offering \"{model.Name}\" was deleted successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ServiceOffering");

        }

        [HttpPost]
        public async Task<JsonResult> EditServiceOfferingAjax(Models.Shared.ServiceOfferingModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidServiceOfferingName(model.Name))
            {
                ModelState.AddModelError("ServiceOffering", "That service offering already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsServiceOfferingInUse(model.Id);

            // Update Service Offering
            await _biobankWriteService.UpdateServiceOfferingAsync(new ServiceOffering
            {
                ServiceId = model.Id,
                Name = model.Name,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult EditServiceOfferingSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The service offering \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ServiceOffering");
        }

        [HttpPost]
        public async Task<JsonResult> AddServiceOfferingAjax(Models.Shared.ServiceOfferingModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidServiceOfferingName(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Service offering names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }


            await _biobankWriteService.AddServiceOfferingAsync(new ServiceOffering
            {
                Name = model.Name,
                SortOrder = model.SortOrder
            });
            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        public ActionResult AddServiceOfferingSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The service offering \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ServiceOffering");
        }

        #endregion

        #endregion

        #region Site Configuration

        #region Homepage Config
        public async Task<ActionResult> HomepageConfig()
        {
            return View(new HomepageContentModel
            {
                Title = Config.Get(ConfigKey.HomepageTitle, ""),
                SearchTitle = Config.Get(ConfigKey.HomepageSearchTitle, ""),
                ResourceRegistration = Config.Get(ConfigKey.HomepageResourceRegistration, ""),
                NetworkRegistration = Config.Get(ConfigKey.HomepageNetworkRegistration, ""),
            });
        }

        [HttpPost]
        public ActionResult HomepageConfig(HomepageContentModel homepage)
            => View(homepage);

        [HttpPost]
        public async Task<ActionResult> SaveHomepageConfig(HomepageContentModel homepage)
        {
            await _biobankWriteService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.HomepageTitle, Value = homepage.Title ?? "" },
                    new Config { Key = ConfigKey.HomepageSearchTitle, Value = homepage.SearchTitle ?? "" },
                    new Config { Key = ConfigKey.HomepageResourceRegistration, Value = homepage.ResourceRegistration ?? "" },
                    new Config { Key = ConfigKey.HomepageNetworkRegistration, Value = homepage.NetworkRegistration ?? "" }
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
        public async Task<ActionResult> TermpageConfig()
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
            await _biobankWriteService.UpdateSiteConfigsAsync(
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
            //Populate Diagnoses for Preview View
            var diagnoses = (await _biobankReadService.ListCollectionsAsync())
                .Where(x => x.SampleSets.Any())
                .GroupBy(x => x.DiagnosisId)
                .Select(x => x.First().Diagnosis);

            var diagnosesModel = new DiagnosesModel
            {
                Diagnoses = diagnoses
                       .Select(x =>
                       Task.Run(async () => new ReadDiagnosisModel
                       {
                           Id = x.DiagnosisId,
                           SnomedIdentifier = x.SnomedIdentifier,
                           Description = x.Description,
                           CollectionCapabilityCount = await _biobankReadService.GetDiagnosisCollectionCapabilityCount(x.DiagnosisId),
                           OtherTerms = x.OtherTerms
                       })
                   .Result
                   )
                   .ToList()
            };
            var diagnosesModels = new List<DiagnosesModel>();
            diagnosesModels.Add(diagnosesModel);

            return View("TermpageConfigPreview", new TermPageModel
            {
                TermpageContentModel = termpage,
                DiagnosesModel = diagnosesModels
            });
        }


        #endregion

        #region Register Biobank Config
        public async Task<ActionResult> RegisterBiobankConfig()
        {
            return View(new RegisterConfigModel
            {
                BiobankDescription = Config.Get(ConfigKey.RegisterBiobankDescription, ""),
                NetworkDescription = Config.Get(ConfigKey.RegisterNetworkDescription, ""),
            });
        }


        [HttpPost]
        public async Task<ActionResult> SaveRegisterBiobankConfig(RegisterConfigModel registerConfigModel)
        {
            await _biobankWriteService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.RegisterBiobankDescription, Value = registerConfigModel.BiobankDescription ?? "" },
                    new Config { Key = ConfigKey.RegisterNetworkDescription, Value = registerConfigModel.NetworkDescription ?? "" },
                }
            );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;
            var sampleResource = await _biobankReadService.GetSiteConfigValue(ConfigKey.SampleResourceName);
            SetTemporaryFeedbackMessage("Register " + sampleResource + " Configuration saved successfully.", FeedbackMessageType.Success);
            return Redirect("RegisterBiobankConfig");
        }
        #endregion

        #region Register Network Config
        public ActionResult RegisterNetworkConfig() => View();

        [HttpPost]
        public async Task<ActionResult> SaveRegisterNetworkConfig(string description)
        {
            await _biobankWriteService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.RegisterNetworkDescription, Value = description },
                }
            );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;

            SetTemporaryFeedbackMessage("Register Network Configuration saved successfully.", FeedbackMessageType.Success);

            return Redirect("RegisterNetworkConfig");
        }
        #endregion

        #region Site Config
        public async Task<ActionResult> SiteConfig()
        {
            return View((await _biobankReadService.ListSiteConfigsAsync("site.display"))
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
            await _biobankWriteService.UpdateSiteConfigsAsync(
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
            return View((await _biobankReadService.ListSiteConfigsAsync("site.sampleresource"))
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
        #region About Page Config
        public async Task<ActionResult> AboutpageConfig()
        {
            if (await _biobankReadService.GetSiteConfigStatus(ConfigKey.DisplayAboutPage) == true)
            {
                return View(new AboutModel
                {
                    BodyText = Config.Get(ConfigKey.AboutBodyText, "")
                });
            }
            else
            {
                return RedirectToAction("LockedRef");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AboutpageConfig(AboutModel aboutpage)
        {
            if (await _biobankReadService.GetSiteConfigStatus(ConfigKey.DisplayAboutPage) == true)
            {
                return View(aboutpage);
            }
            else
            {
                return RedirectToAction("LockedRef");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveAboutpageConfig(AboutModel aboutpage)
        {
            await _biobankWriteService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.AboutBodyText, Value = aboutpage.BodyText },
                }
            );

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;
            SetTemporaryFeedbackMessage("About page body text saved successfully.", FeedbackMessageType.Success);
            return Redirect("AboutpageConfig");
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
            await _biobankWriteService.UpdateSiteConfigsAsync(values);

            // Invalidate current config (Refreshed in SiteConfigAttribute filter)
            HttpContext.Application["Config"] = null;
            SetTemporaryFeedbackMessage("The Reference Term has been overriden successfully.", FeedbackMessageType.Success);
            return Json(new
            {
                success = true,
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
