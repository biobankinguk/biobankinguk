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
using Directory.Services;
using Microsoft.Ajax.Utilities;
using Hangfire.States;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json.Linq;

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
        private readonly HttpClient _client;

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

            //Http client for RefData Apis
            _client = new HttpClient()
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["RefDataApiUrl"])
            };
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
                AccessConditionId = model.Id
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

        public ActionResult EditAssociatedDataProcurementTimeFrameSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data procurement time frame \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataProcurementTimeFrame");
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
            var endpoint = "api/MaterialTypes/MaterialTypes";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadMaterialTypeModel>>(contents);
                return View(new MaterialTypesModel
                {
                    MaterialTypes = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new MaterialTypesModel { MaterialTypes = new List<ReadMaterialTypeModel> { } });
            }

        }

        public async Task<ActionResult> DeleteMaterialType(MaterialTypeModel model)
        {
            var endpoint = "api/MaterialTypes/DeleteMaterialType";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("MaterialTypes");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("MaterialTypes");
            }
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
            var endpoint = "api/DiseaseStatuses/DiseaseStatuses";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadDiagnosisModel>>(contents);
                return View(new DiagnosesModel
                {
                    Diagnoses = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new DiagnosesModel { Diagnoses = new List<ReadDiagnosisModel> { } });
            }
        }

        public async Task<ActionResult> DeleteDiseaseStatus(DiagnosisModel model)
        {
            var endpoint = "api/DiseaseStatuses/DeleteDiseaseStatus";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("DiseaseStatuses");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("DiseaseStatuses");
            }
        }

        public ActionResult EditDiseaseStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The disease status \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("DiseaseStatuses");
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
            var endpoint = "api/CollectionPercentages/CollectionPercentages";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<CollectionPercentageModel>>(contents);
                if (await _biobankReadService.GetSiteConfigStatus("site.display.preservation.percent") == true)
                {
                    return View(new CollectionPercentagesModel()
                    {
                        CollectionPercentages = result
                    });
                }
                else
                {
                    return RedirectToAction("LockedRef");
                }
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new CollectionPercentagesModel { CollectionPercentages = new List<CollectionPercentageModel> { } });
            }

            
        }

        public async Task<ActionResult> DeleteCollectionPercentage(CollectionPercentageModel model)
        {
            var endpoint = "api/CollectionPercentages/DeleteCollectionPercentage";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("CollectionPercentages");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("CollectionPercentages");
            }
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
            var endpoint = "api/CollectionPoints/CollectionPoints";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<CollectionPointModel>>(contents);
                return View(new CollectionPointsModel()
                {
                    CollectionPoints = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new CollectionPointsModel { CollectionPoints = new List<CollectionPointModel> { } });
            }
        }

        public async Task<ActionResult> DeleteCollectionPoint(CollectionPointModel model)
        {
            var endpoint = "api/CollectionPoints/DeleteCollectionPoint";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("CollectionPoints");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("CollectionPoints");
            }
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
            var endpoint = "api/DonorCounts/DonorCounts";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<DonorCountModel>>(contents);
                return View(new DonorCountsModel()
                {
                    DonorCounts = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new DonorCountsModel { DonorCounts = new List<DonorCountModel> { } });
            }
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
            var endpoint = "api/DonorCounts/DeleteDonorCount";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("DonorCounts");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("DonorCounts");
            }
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
            var endpoint = "api/CollectionType/CollectionType";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadCollectionTypeModel>>(contents);
                return View(new Models.ADAC.CollectionTypeModel
                {
                    CollectionTypes = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new Models.ADAC.CollectionTypeModel { CollectionTypes = new List<ReadCollectionTypeModel> { } });
            }

        }

        public async Task<ActionResult> DeleteCollectionType(Models.Shared.CollectionTypeModel model)
        {
            var endpoint = "api/CollectionType/DeleteCollectionType";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("CollectionType");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("CollectionType");
            }

        }

        public ActionResult EditCollectionTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The collection type \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionType");
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
            var endpoint = "api/PreservationTypes/PreservationTypes";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<PreservationTypeModel>>(contents);
                return View(new PreservationTypesModel
                {
                    PreservationTypes = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new PreservationTypesModel { PreservationTypes = new List<PreservationTypeModel> { } });
            }
        }

        public async Task<ActionResult> DeletePreservationType(PreservationTypeModel model)
        {
            var endpoint = "api/PreservationTypes/DeletePreservationType";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("PreservationTypes");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("PreservationTypes");
            }
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
            var endpoint = "api/AssociatedDataTypes/AssociatedDataTypes";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);
                return View(new AssociatedDataTypesModel
                {
                    AssociatedDataTypes = result["AssociatedDataTypes"].ToObject<IList<AssociatedDataTypeModel>>(),
                    AssociatedDataTypeGroups = result["AssociatedDataTypeGroups"].ToObject<IList<AssociatedDataTypeGroupModel>>()
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new AssociatedDataTypesModel
                { 
                    AssociatedDataTypes = new List<AssociatedDataTypeModel> { },
                    AssociatedDataTypeGroups = new List<AssociatedDataTypeGroupModel> { }
                });
            }
        }

        public async Task<ActionResult> DeleteAssociatedDataType(AssociatedDataTypeModel model)
        {
            var endpoint = "api/AssociatedDataTypes/DeleteAssociatedDataType";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("AssociatedDataTypes");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("AssociatedDataTypes");
            }
        }

        public ActionResult EditAssociatedDataTypeSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data type \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypes");
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
            var endpoint = "api/AssociatedDataTypeGroups/AssociatedDataTypeGroups";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadAssociatedDataTypeGroupModel>>(contents);
                return View(new AssociatedDataTypesGroupModel
                {
                    AssociatedDataTypeGroups = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new AssociatedDataTypesGroupModel { AssociatedDataTypeGroups = new List<ReadAssociatedDataTypeGroupModel> { } });
            }
        }
        public async Task<ActionResult> DeleteAssociatedDataTypeGroup(AssociatedDataTypeGroupModel model)
        {
            var endpoint = "api/AssociatedDataTypeGroups/DeleteAssociatedDataTypeGroup";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("AssociatedDataTypeGroups");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("AssociatedDataTypeGroups");
            }
        }
       
        public ActionResult AddAssociatedDataTypeGroupSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The associated data type group \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AssociatedDataTypeGroups");
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
            var endpoint = "api/ConsentRestrictions/ConsentRestriction";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadConsentRestrictionModel>>(contents);
                return View(new Models.ADAC.ConsentRestrictionModel
                {
                    ConsentRestrictions = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new AccessConditionsModel { AccessConditions = new List<ReadAccessConditionsModel> { } });
            }
        }

        public async Task<ActionResult> DeleteConsentRestriction(Models.Shared.ConsentRestrictionModel model)
        {
            var endpoint = "api/ConsentRestrictions/DeleteConsentRestriction";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("ConsentRestriction");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("ConsentRestriction");
            }
        }

        public ActionResult EditConsentRestrictionSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The consent restriction \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ConsentRestriction");
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
            var endpoint = "api/CollectionStatus/CollectionStatus";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadCollectionStatusModel>>(contents);
                return View(new Models.ADAC.CollectionStatusModel
                {
                    CollectionStatuses = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new Models.ADAC.CollectionStatusModel { CollectionStatuses = new List<ReadCollectionStatusModel> { } });
            }
        }

        public async Task<ActionResult> DeleteCollectionStatus(Models.Shared.CollectionStatusModel model)
        {
            var endpoint = "api/CollectionStatus/DeleteCollectionStatus";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("CollectionStatus");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("CollectionStatus");
            }
        }

        public ActionResult EditCollectionStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The collection status \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("CollectionStatus");
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
            var endpoint = "api/AnnualStatisticGroups/AnnualStatisticGroups";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadAnnualStatisticGroupModel>>(contents);
                return View(new AnnualStatisticGroupsModel
                {
                    AnnualStatisticGroups = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new AnnualStatisticGroupsModel { AnnualStatisticGroups = new List<ReadAnnualStatisticGroupModel> { } });
            }
        }

        public async Task<ActionResult> DeleteAnnualStatisticGroup(AnnualStatisticGroupModel model)
        {
            var endpoint = "api/AnnualStatisticGroups/DeleteAnnualStatisticGroup";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("AnnualStatisticGroups");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("AnnualStatisticGroups");
            }
        }

        public ActionResult EditAnnualStatisticGroupSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The annual statistic group \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("AnnualStatisticGroups");
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
            var endpoint = "api/SampleCollectionModes/SampleCollectionModes";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<SampleCollectionModeModel>>(contents);
                return View(new SampleCollectionModesModel
                {
                    SampleCollectionModes = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new SampleCollectionModesModel { SampleCollectionModes = new List<SampleCollectionModeModel> { } });
            }
        }

        public async Task<ActionResult> DeleteSampleCollectionMode(SampleCollectionModeModel model)
        {
            var endpoint = "api/SampleCollectionModes/DeleteSampleCollectionMode";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("SampleCollectionModes");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("SampleCollectionModes");
            }
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
            var endpoint = "api/Sexes/Sexes";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadSexModel>>(contents);
                return View(new SexesModel
                {
                    Sexes = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new SexesModel { Sexes = new List<ReadSexModel> { } });
            }
        }

        public async Task<ActionResult> DeleteSex(SexModel model)
        {
            var endpoint = "api/Sexes/DeleteSex";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("Sexes");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("Sexes");
            }
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
            var endpoint = "api/Country/Country";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadCountryModel>>(contents);
                return View(new Models.ADAC.CountryModel
                {
                    Countries = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new Models.ADAC.CountryModel { Countries = new List<ReadCountryModel> { } });
            }
        }

        public ActionResult AddCountrySuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The country \"{name}\" has been added successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("Country");
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
            var endpoint = "api/Country/DeleteCountry";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("Country");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("Country");
            }
        }
        #endregion

        #region RefData: County
        public async Task<ActionResult> County()
        {
            if (await _biobankReadService.GetSiteConfigStatus("site.display.counties") == true)
            {
                var endpoint = "api/County/County";
                try
                {
                    //Make request
                    var response = await _client.GetAsync(endpoint);
                    var contents = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<CountiesModel>(contents);
                    return View(result);
                }
                catch (Exception)
                {
                    SetTemporaryFeedbackMessage($"Something went wrong!",
                        FeedbackMessageType.Danger);
                    return View(new CountiesModel { });
                }
            }
            else
            {
                return RedirectToAction("LockedRef");
            }
        }

        public async Task<ActionResult> DeleteCounty(CountyModel model)
        {
            var endpoint = "api/County/DeleteCounty";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("Country");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("Country");
            }
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
            var endpoint = "api/SopStatus/SopStatus";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<SopStatusModel>>(contents);
                return View(new SopStatusesModel
                {
                    SopStatuses = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new SopStatusesModel { SopStatuses = new List<SopStatusModel> { } });
            }
        }

        public async Task<ActionResult> DeleteSopStatus(SopStatusModel model)
        {
            var endpoint = "api/SopStatus/DeleteSopStatus";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("SopStatus");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("SopStatus");
            }
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
            var endpoint = "api/RegistrationReason/RegistrationReason";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadRegistrationReasonModel>>(contents);
                return View(new Models.ADAC.RegistrationReasonModel
                {
                    RegistrationReasons = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new Models.ADAC.RegistrationReasonModel { RegistrationReasons = new List<ReadRegistrationReasonModel> { } });
            }
        }

        public async Task<ActionResult> DeleteRegistrationReason(Models.Shared.RegistrationReasonModel model)
        {
            var endpoint = "api/RegistrationReason/DeleteRegistrationReason";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("RegistrationReason");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("RegistrationReason");
            }
        }

        public ActionResult EditRegistrationReasonSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The registration reason \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("RegistrationReason");
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
            var endpoint = "api/MacroscopicAssessments/MacroscopicAssessments";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<MacroscopicAssessmentModel>>(contents);
                return View(new MacroscopicAssessmentsModel
                {
                    MacroscopicAssessments = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new MacroscopicAssessmentsModel { MacroscopicAssessments = new List<MacroscopicAssessmentModel> { } });
            }
        }

        public async Task<ActionResult> DeleteMacroscopicAssessment(MacroscopicAssessmentModel model)
        {
            var endpoint = "api/MacroscopicAssessments/DeleteMacroscopicAssessment";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("MacroscopicAssessments");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("MacroscopicAssessments");
            }
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
                var endpoint = "api/HtaStatus/HtaStatus";
                try
                {
                    //Make request
                    var response = await _client.GetAsync(endpoint);
                    var contents = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<IList<ReadHtaStatusModel>>(contents);
                    return View(new Models.ADAC.HtaStatusModel
                    {
                        HtaStatuses = result
                    });
                }
                catch (Exception)
                {
                    SetTemporaryFeedbackMessage($"Something went wrong!",
                        FeedbackMessageType.Danger);
                    return View(new Models.ADAC.HtaStatusModel { HtaStatuses = new List<ReadHtaStatusModel> { } });
                }
            }
            else
            {
                return RedirectToAction("LockedRef");
            }
        }


        public async Task<ActionResult> DeleteHtaStatus(Models.Shared.HtaStatusModel model)
        {
            var endpoint = "api/HtaStatus/DeleteHtaStatus";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("HtaStatus");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("HtaStatus");
            }
        }

        public ActionResult EditHtaStatusSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The hta status \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("HtaStatus");
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
            var endpoint = "api/ServiceOfferings/ServiceOffering";
            try
            {
                //Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IList<ReadServiceOfferingModel>>(contents);
                return View(new Models.ADAC.ServiceOfferingModel
                {
                    ServiceOfferings = result
                });
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);
                return View(new Models.ADAC.ServiceOfferingModel { ServiceOfferings = new List<ReadServiceOfferingModel> { } });
            }
        }
        public async Task<ActionResult> DeleteServiceOffering(Models.Shared.ServiceOfferingModel model)
        {
            var endpoint = "api/ServiceOfferings/DeleteServiceOffering";
            try
            {
                //Make request
                var response = await _client.PostAsJsonAsync(endpoint, model);
                var contents = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(contents);

                //Everything went A-OK!
                SetTemporaryFeedbackMessage(result["msg"].ToString(),
                    (FeedbackMessageType)int.Parse(result["type"].ToString()));

                return RedirectToAction("ServiceOffering");
            }
            catch (Exception)
            {
                SetTemporaryFeedbackMessage($"Something went wrong!",
                    FeedbackMessageType.Danger);

                return RedirectToAction("ServiceOffering");
            }
        }

        public ActionResult EditServiceOfferingSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"The service offering \"{name}\" has been edited successfully.",
                FeedbackMessageType.Success);

            return RedirectToAction("ServiceOffering");
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
            await _biobankWriteService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.HomepageTitle, Value = homepage.Title ?? "" },
                    new Config { Key = ConfigKey.HomepageSearchTitle, Value = homepage.SearchTitle ?? "" },
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

        #region Register Biobank and Network Pages Config
        public async Task<ActionResult> RegisterPagesConfig()
        {
            return View(new RegisterConfigModel
            {
                BiobankTitle = Config.Get(ConfigKey.RegisterBiobankTitle, ""),
                BiobankDescription = Config.Get(ConfigKey.RegisterBiobankDescription, ""),
                NetworkTitle = Config.Get(ConfigKey.RegisterNetworkTitle, ""),
                NetworkDescription = Config.Get(ConfigKey.RegisterNetworkDescription, ""),
            });
        }


        [HttpPost]
        public async Task<ActionResult> SaveRegisterPagesConfig(RegisterConfigModel registerConfigModel)
        {
            await _biobankWriteService.UpdateSiteConfigsAsync(
                new List<Config>
                {
                    new Config { Key = ConfigKey.RegisterBiobankTitle, Value = registerConfigModel.BiobankTitle ?? ""},
                    new Config { Key = ConfigKey.RegisterBiobankDescription, Value = registerConfigModel.BiobankDescription ?? "" },
                    new Config { Key = ConfigKey.RegisterNetworkTitle, Value = registerConfigModel.NetworkTitle ?? ""},
                    new Config { Key = ConfigKey.RegisterNetworkDescription, Value = registerConfigModel.NetworkDescription ?? "" },
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
