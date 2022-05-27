using AutoMapper;
using Biobanks.Directory.Data.Constants;
using Biobanks.Directory.Data.Transforms.Url;
using Biobanks.Directory.Services.Constants;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Identity.Constants;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Biobanks.Web.Extensions;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.Biobank;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;

using Microsoft.ApplicationInsights;
using Microsoft.AspNet.Identity;

using MvcSiteMapProvider;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using static System.String;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "BiobankAdmin")]
    [SuspendedWarning]
    public class BiobankController : ApplicationBaseController
    {
        private readonly ICollectionService _collectionService;
        private readonly IPublicationService _publicationService;

        private readonly IReferenceDataService<ServiceOffering> _serviceOfferingService;
        private readonly IReferenceDataService<RegistrationReason> _registrationReasonService;
        private readonly IReferenceDataService<MacroscopicAssessment> _macroscopicAssessmentService;
        private readonly IReferenceDataService<DonorCount> _donorCountService;
        private readonly IReferenceDataService<County> _countyService;
        private readonly IReferenceDataService<Country> _countryService;
        private readonly IReferenceDataService<ConsentRestriction> _consentRestrictionService;
        private readonly IReferenceDataService<CollectionType> _collectionTypeService;
        private readonly IReferenceDataService<CollectionPercentage> _collectionPercentageService;
        private readonly IReferenceDataService<AnnualStatisticGroup> _annualStatisticGroupService;
        private readonly IReferenceDataService<AgeRange> _ageRangeService;
        private readonly IReferenceDataService<CollectionStatus> _collectionStatusService;
        private readonly IReferenceDataService<AccessCondition> _accessConditionService;
        private readonly IReferenceDataService<Funder> _funderService;
        private readonly IReferenceDataService<Sex> _sexService;
        private readonly IReferenceDataService<PreservationType> _preservationTypeService;
        private readonly IReferenceDataService<StorageTemperature> _storageTemperatureService;
        private readonly IReferenceDataService<MaterialType> _materialTypeService;
        private readonly IReferenceDataService<AssociatedDataType> _associatedDataTypeService;
        private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
        private readonly IReferenceDataService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeframeService;

        private readonly IOntologyTermService _ontologyTermService;

        private readonly INetworkService _networkService;
        private readonly IOrganisationService _organisationService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IConfigService _configService;
        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;

        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IEmailService _emailService;
        private readonly CustomClaimsManager _claimsManager;

        private readonly IMapper _mapper;
        private readonly ITokenLoggingService _tokenLog;

        private const string TempBiobankLogoSessionId = "TempBiobankLogo";
        private const string TempBiobankLogoContentTypeSessionId = "TempBiobankLogoContentType";

        public BiobankController(
            ICollectionService collectionService,
            IPublicationService publicationService,
            IReferenceDataService<ServiceOffering> serviceOfferingService,
            IReferenceDataService<RegistrationReason> registrationReasonService,
            IReferenceDataService<MacroscopicAssessment> macroscopicAssessmentService,
            IReferenceDataService<DonorCount> donorCountService,
            IReferenceDataService<County> countyService,
            IReferenceDataService<Country> countryService,
            IReferenceDataService<ConsentRestriction> consentRestrictionService,
            IReferenceDataService<CollectionType> collectionTypeService,
            IReferenceDataService<CollectionPercentage> collectionPercentageService,
            IReferenceDataService<AnnualStatisticGroup> annualStatisticGroupService,
            IReferenceDataService<AgeRange> ageRangeService,
            IReferenceDataService<AccessCondition> accessConditionService,
            IReferenceDataService<CollectionStatus> collectionStatusService,
            IReferenceDataService<Funder> funderService,
            IReferenceDataService<Sex> sexService,
            IReferenceDataService<PreservationType> preservationTypeService,
            IReferenceDataService<StorageTemperature> storageTemperatureService,
            IReferenceDataService<MaterialType> materialTypeService,
            IReferenceDataService<AssociatedDataType> assocaitedDataTypeService,
            IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
            IReferenceDataService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeframeService,
            IOntologyTermService ontologyTermService,
            INetworkService networkService,
            IOrganisationService organisationService,
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IConfigService configService,
            IAnalyticsReportGenerator analyticsReportGenerator,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IEmailService emailService,
            IMapper mapper,
            CustomClaimsManager claimsManager,
            ITokenLoggingService tokenLog)
        {
            _collectionService = collectionService;
            _publicationService = publicationService;
            _serviceOfferingService = serviceOfferingService;
            _registrationReasonService = registrationReasonService;
            _macroscopicAssessmentService = macroscopicAssessmentService;
            _donorCountService = donorCountService;
            _countyService = countyService;
            _countryService = countryService;
            _consentRestrictionService = consentRestrictionService;
            _collectionService = collectionService;
            _collectionTypeService = collectionTypeService;
            _collectionPercentageService = collectionPercentageService;
            _annualStatisticGroupService = annualStatisticGroupService;
            _ageRangeService = ageRangeService;
            _accessConditionService = accessConditionService;
            _collectionStatusService = collectionStatusService;
            _funderService = funderService;
            _sexService = sexService;
            _preservationTypeService = preservationTypeService;
            _storageTemperatureService = storageTemperatureService;
            _materialTypeService = materialTypeService;
            _associatedDataTypeService = assocaitedDataTypeService;
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
            _associatedDataProcurementTimeframeService = associatedDataProcurementTimeframeService;
            _ontologyTermService = ontologyTermService;
            _networkService = networkService;
            _organisationService = organisationService;
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _configService = configService;
            _analyticsReportGenerator = analyticsReportGenerator;
            _userManager = userManager;
            _emailService = emailService;
            _mapper = mapper;
            _claimsManager = claimsManager;
            _tokenLog = tokenLog;
        }

        #region Biobank details

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> Index()
        {
            var model = await GetBiobankDetailsModelAsync();

            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            //for viewing details only, we include networks
            var networks = await _networkService.ListByOrganisationId(biobankId);

            model.AnnualStatisticGroups = await _annualStatisticGroupService.List();

            model.NetworkModels = networks.Select(x => new NetworkMemberModel
            {
                Id = x.NetworkId,
                Logo = x.Logo,
                Name = x.Name
            }).ToList();

            return View(model);
        }

        private async Task<Organisation> EditBiobankDetails(BiobankDetailsModel model)
        {
            if (model.BiobankId == null) throw new ApplicationException();

            var logoName = model.LogoName;

            // if updating, try and upload a logo now (ensuring logoName is correct)
            if (model.RemoveLogo)
            {
                logoName = null;
                await _biobankWriteService.RemoveLogoAsync(model.BiobankId.Value);
            }
            else if (model.Logo != null)
            {
                try
                {
                    var logoStream = model.Logo.ToProcessableStream();

                    logoName =
                        await
                            _biobankWriteService.StoreLogoAsync(
                                logoStream,
                                model.Logo.FileName,
                                model.Logo.ContentType,
                                model.BiobankExternalId);
                }
                catch (ArgumentNullException)
                {
                } //no problem, just means no logo uploaded in this form submission
            }

            var biobank = _mapper.Map<Organisation>(model);

            //Update bits Automapper doesn't do
            biobank.OrganisationRegistrationReasons = new List<OrganisationRegistrationReason>();
            biobank.OrganisationServiceOfferings = new List<OrganisationServiceOffering>();

            foreach (var rr in model.RegistrationReasons)
            {
                if (rr.Active)
                    biobank.OrganisationRegistrationReasons.Add(new OrganisationRegistrationReason
                    { OrganisationId = biobank.OrganisationId, RegistrationReasonId = rr.RegistrationReasonId });
            }

            foreach (var sm in model.ServiceModels)
            {
                if (sm.Active)
                    biobank.OrganisationServiceOfferings.Add(new OrganisationServiceOffering
                    { OrganisationId = biobank.OrganisationId, ServiceOfferingId = sm.ServiceOfferingId });
            }

            biobank.Logo = logoName;
            return await _organisationService.Update(biobank);
        }

        private async Task<Organisation> CreateBiobank(BiobankDetailsModel model)
        {
            var biobank = await _organisationService.Create(_mapper.Map<OrganisationDTO>(model));
            await _organisationService.AddUserToOrganisation(User.Identity.GetUserId(), biobank.OrganisationId);

            //update the request to show org created
            var request = await _organisationService.GetRegistrationRequestByEmail(User.Identity.Name);
            request.OrganisationCreatedDate = DateTime.Now;
            request.OrganisationExternalId = biobank.OrganisationExternalId;
            await _organisationService.UpdateRegistrationRequest(request);

            //add a claim now that they're associated with the biobank
            _claimsManager.AddClaims(new List<Claim>
                    {
                        new Claim(CustomClaimType.Biobank, JsonConvert.SerializeObject(new KeyValuePair<int, string>(biobank.OrganisationId, biobank.Name)))
                    });

            Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.Biobank;
            Session[SessionKeys.ActiveOrganisationId] = biobank.OrganisationId;
            Session[SessionKeys.ActiveOrganisationName] = biobank.Name;

            //Logo upload (now we have the id, we can form the filename)
            if (model.Logo != null)
            {
                try
                {
                    var logoStream = model.Logo.ToProcessableStream();

                    //use the DTO again to update
                    biobank.Logo = await
                                _biobankWriteService.StoreLogoAsync(logoStream,
                                    model.Logo.FileName,
                                    model.Logo.ContentType,
                                    biobank.OrganisationExternalId);

                    biobank = await _organisationService.Update(biobank);
                }
                catch (ArgumentNullException)
                {
                } //no problem, just means no logo uploaded in this form submission

            }


            return biobank;
        }

        public async Task<ActionResult> Edit(bool detailsIncomplete = false)
        {
            var sampleResource = await _configService.GetSiteConfigValue(ConfigKey.SampleResourceName);

            if (detailsIncomplete)
                SetTemporaryFeedbackMessage("Please fill in the details below for your " + sampleResource + ". Once you have completed these, you'll be able to perform other administration tasks",
                    FeedbackMessageType.Info);

            var activeOrganisationType = Convert.ToInt32(Session[SessionKeys.ActiveOrganisationType]);

            return activeOrganisationType == (int)ActiveOrganisationType.NewBiobank
                ? View(await NewBiobankDetailsModelAsync()) //no biobank id means we're dealing with a request
                : View(await GetBiobankDetailsModelAsync()); //biobank id means we're dealing with an existing biobank
        }

        private async Task<BiobankDetailsModel> AddCountiesToModel(BiobankDetailsModel model)
        {
            model.Counties = await _countyService.List();
            model.Countries = await _countryService.List();
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BiobankDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await AddCountiesToModel(model);
                return View(model);
            }

            //Extra form validation that the model state can't do for us

            //Logo, if any
            try
            {
                model.Logo.ValidateAsLogo();
            }
            catch (BadImageFormatException ex)
            {
                model = await AddCountiesToModel(model);
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            //URL, if any
            try
            {
                model.Url = UrlTransformer.Transform(model.Url);
            }
            catch (InvalidUrlSchemeException e)
            {
                model = await AddCountiesToModel(model);
                ModelState.AddModelError("", e.Message);
                model = await AddCountiesToModel(model);
                return View(model);
            }
            catch (UriFormatException)
            {
                model = await AddCountiesToModel(model);
                ModelState.AddModelError("",
                    $"{model.Url} is not a valid URL.");
                model = await AddCountiesToModel(model);
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                try
                {
                    // Attempt to access the page and see if it returns 2xx code
                    // Automatically follows redirects (though the RFC says .NET Framework is being naughty by doing this!)
                    var response = await new HttpClient().GetAsync(model.Url);
                    if (!response.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError(Empty, $"{model.Url} does not appear to be a valid URL.");
                        model = await AddCountiesToModel(model);
                        return View(model);
                    }
                }
                catch
                {
                    ModelState.AddModelError(Empty, $"Could not access URL {model.Url}.");
                    model = await AddCountiesToModel(model);
                    return View(model);
                }
            }

            //Create or Edit the biobank details
            Organisation biobank;
            if (model.BiobankId == null) biobank = await CreateBiobank(model);
            else biobank = await EditBiobankDetails(model);


            //stuff that happens regardless of create/update (but requires id to be populated, so has to be done after the above)

            //Add/Delete Services
            var activeServices =
                model.ServiceModels.Where(x => x.Active).Select(x => new OrganisationServiceOffering
                {
                    ServiceOfferingId = x.ServiceOfferingId,
                    OrganisationId = biobank.OrganisationId
                }).ToList();

            await _biobankWriteService.AddBiobankServicesAsync(activeServices);

            foreach (var inactiveService in model.ServiceModels.Where(x => !x.Active))
            {
                await
                    _biobankWriteService.DeleteBiobankServiceAsync(biobank.OrganisationId,
                        inactiveService.ServiceOfferingId);
            }

            //Add/Delete registration reasons
            var activeRegistrationReasons =
                model.RegistrationReasons.Where(x => x.Active).Select(x => new OrganisationRegistrationReason
                {
                    RegistrationReasonId = x.RegistrationReasonId,
                    OrganisationId = biobank.OrganisationId
                }).ToList();

            await _biobankWriteService.AddBiobankRegistrationReasons(activeRegistrationReasons);

            foreach (var inactiveRegistrationReason in model.RegistrationReasons.Where(x => !x.Active))
            {
                await
                    _biobankWriteService.DeleteBiobankRegistrationReasonAsync(biobank.OrganisationId,
                        inactiveRegistrationReason.RegistrationReasonId);
            }
            var sampleResource = await _configService.GetSiteConfigValue(ConfigKey.SampleResourceName);
            SetTemporaryFeedbackMessage(sampleResource + " details updated!", FeedbackMessageType.Success);

            //Back to the profile to view your saved changes
            return RedirectToAction("Index");
        }

        private async Task<List<OrganisationServiceModel>> GetAllServicesAsync()
        {
            var allServices = await _serviceOfferingService.List();

            return allServices.Select(service => new OrganisationServiceModel
            {
                Active = false,
                ServiceOfferingName = service.Value,
                ServiceOfferingId = service.Id,
                SortOrder = service.SortOrder
            })
            .OrderBy(x => x.SortOrder)
            .ToList();
        }

        private async Task<List<OrganisationRegistrationReasonModel>> GetAllRegistrationReasonsAsync()
        {
            var allRegistrationReasons = await _registrationReasonService.List();

            return allRegistrationReasons.Select(regReason => new OrganisationRegistrationReasonModel
            {
                Active = false,
                RegistrationReasonName = regReason.Value,
                RegistrationReasonId = regReason.Id
            })
            .ToList();
        }

        private async Task<BiobankDetailsModel> NewBiobankDetailsModelAsync()
        {
            //the biobank doesn't exist yet, but a request should, so we can get the name
            var request = await _organisationService.GetRegistrationRequest(SessionHelper.GetBiobankId(Session));

            //validate that the request is accepted
            if (request.AcceptedDate == null) return null;

            var model = new BiobankDetailsModel
            {
                OrganisationName = request.OrganisationName,
                ServiceModels = await GetAllServicesAsync(),
                RegistrationReasons = await GetAllRegistrationReasonsAsync(),
                Counties = await _countyService.List(),
                Countries = await _countryService.List()
            };

            model = await AddCountiesToModel(model);

            return model;
        }

        private async Task<BiobankDetailsModel> GetBiobankDetailsModelAsync()
        {
            //having a biobankId claim means we can definitely get a biobank for that claim and return a model for that
            var bb = await _organisationService.Get(SessionHelper.GetBiobankId(Session));

            //Try and get any service offerings for this biobank
            var bbServices =
                    await _biobankReadService.ListBiobankServiceOfferingsAsync(bb.OrganisationId);

            //mark services as active in the full list
            var services = (await GetAllServicesAsync()).Select(service =>
            {
                service.Active = bbServices.Any(x => x.ServiceOfferingId == service.ServiceOfferingId);
                return service;
            }).ToList();

            //Try and get any registration reasons for this biobank
            var bbRegistrationReasons =
                    await _organisationService.ListRegistrationReasons(bb.OrganisationId);

            //mark services as active in the full list
            var registrationReasons = (await GetAllRegistrationReasonsAsync()).Select(regReason =>
            {
                regReason.Active = bbRegistrationReasons.Any(x => x.RegistrationReasonId == regReason.RegistrationReasonId);
                return regReason;
            }).ToList();

            var model = new BiobankDetailsModel
            {
                BiobankId = bb.OrganisationId,
                BiobankExternalId = bb.OrganisationExternalId,
                OrganisationTypeId = bb.OrganisationTypeId,
                AccessConditionId = bb.AccessConditionId,
                CollectionTypeId = bb.CollectionTypeId,
                OrganisationName = bb.Name,
                Description = bb.Description,
                Url = bb.Url,
                LogoName = bb.Logo,
                ContactEmail = bb.ContactEmail,
                ContactNumber = bb.ContactNumber,
                AddressLine1 = bb.AddressLine1,
                AddressLine2 = bb.AddressLine2,
                AddressLine3 = bb.AddressLine3,
                AddressLine4 = bb.AddressLine4,
                City = bb.City,
                CountyId = bb.County?.Id,
                CountryId = bb.Country.Id,
                CountyName = bb.County?.Value,
                CountryName = bb.Country.Value,
                Postcode = bb.PostCode,
                GoverningInstitution = bb.GoverningInstitution,
                GoverningDepartment = bb.GoverningDepartment,
                ServiceModels = services,
                RegistrationReasons = registrationReasons,
                Counties = await _countyService.List(),
                Countries = await _countryService.List(),
                SharingOptOut = bb.SharingOptOut,
                EthicsRegistration = bb.EthicsRegistration,
                BiobankAnnualStatistics = bb.OrganisationAnnualStatistics,
                OtherRegistrationReason = bb.OtherRegistrationReason
            };

            model = await AddCountiesToModel(model);

            return model;
        }

        #endregion

        #region Temp Logo Management

        [HttpPost]
        public JsonResult AddTempLogo()
        {
            if (!System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

            var fileBase = System.Web.HttpContext.Current.Request.Files["TempLogo"];

            if (fileBase == null)
                return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

            if (fileBase.ContentLength > 1000000)
                return Json(new KeyValuePair<bool, string>(false, "The file you supplied is too large. Logo image files must be 1Mb or less."));

            var fileBaseWrapper = new HttpPostedFileWrapper(fileBase);

            try
            {
                if (fileBaseWrapper.ValidateAsLogo())
                {
                    var logoStream = fileBaseWrapper.ToProcessableStream();
                    Session[TempBiobankLogoSessionId] =
                        ImageService.ResizeImageStream(logoStream, maxX: 300, maxY: 300)
                        .ToArray();
                    Session[TempBiobankLogoContentTypeSessionId] = fileBaseWrapper.ContentType;

                    return
                        Json(new KeyValuePair<bool, string>(true,
                            Url.Action("TempLogo", "Biobank")));
                }
            }
            catch (BadImageFormatException e)
            {
                return Json(new KeyValuePair<bool, string>(false, e.Message));
            }

            return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));
        }

        [HttpGet]
        public ActionResult TempLogo(string id)
        {
            return File((byte[])Session[TempBiobankLogoSessionId], Session[TempBiobankLogoContentTypeSessionId].ToString());
        }

        [HttpPost]
        [UserAuthorize(Roles = "BiobankAdmin")]
        public void RemoveTempLogo()
        {
            Session[TempBiobankLogoSessionId] = null;
            Session[TempBiobankLogoContentTypeSessionId] = null;
        }

        #endregion

        #region Admins

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> Admins()
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            return View(new BiobankAdminsModel
            {
                BiobankId = biobankId,
                Admins = await GetAdminsAsync(biobankId, excludeCurrentUser: true)
            });
        }

        private async Task<List<RegisterEntityAdminModel>> GetAdminsAsync(int biobankId, bool excludeCurrentUser)
        {
            //we exclude the current user when we are making the list for them
            //but we may want the full list in other circumstances

            var admins =
                (await _biobankReadService.ListBiobankAdminsAsync(biobankId))
                    .Select(bbAdmin => new RegisterEntityAdminModel
                    {
                        UserId = bbAdmin.Id,
                        UserFullName = bbAdmin.Name,
                        UserEmail = bbAdmin.Email,
                        EmailConfirmed = bbAdmin.EmailConfirmed
                    }).ToList();

            if (excludeCurrentUser)
            {
                admins.Remove(admins.FirstOrDefault(x => x.UserId == CurrentUser.Identity.GetUserId()));
            }

            return admins;
        }

        public async Task<JsonResult> GetAdminsAjax(int biobankId, bool excludeCurrentUser = false, int timeStamp = 0)
        {
            //timeStamp can be used to avoid caching issues, notably on IE

            return Json(await GetAdminsAsync(biobankId, excludeCurrentUser), JsonRequestBehavior.AllowGet);
        }

        public ActionResult InviteAdminSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"{name} has been successfully added to your admins!",
                FeedbackMessageType.Success);

            return RedirectToAction("Admins");
        }

        public async Task<ActionResult> InviteAdminAjax(int biobankId)
        {
            var bb = await _organisationService.Get(biobankId);

            return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
            {
                Entity = bb.Name,
                EntityName = "biobank",
                ControllerName = "Biobank"
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

            var biobankId = (await _organisationService.GetByName(model.Entity)).OrganisationId;
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
                    Url.Action("Index", "Biobank", null, Request.Url.Scheme));
            }

            //Add the user/biobank relationship
            await _organisationService.AddUserToOrganisation(user.Id, biobankId);

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

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> DeleteAdmin(string biobankUserId, string userFullName)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            //remove them from the network
            await _organisationService.RemoveUserFromOrganisation(biobankUserId, biobankId);

            //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
            await _userManager.RemoveFromRolesAsync(biobankUserId, Role.BiobankAdmin.ToString());

            SetTemporaryFeedbackMessage($"{userFullName} has been removed from your admins!", FeedbackMessageType.Success);

            return RedirectToAction("Admins");
        }

        #endregion

        #region Funders

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> Funders()
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            return View(new BiobankFundersModel()
            {
                BiobankId = biobankId,
                Funders = await GetFundersAsync(biobankId)
            });
        }

        private async Task<List<FunderModel>> GetFundersAsync(int biobankId)
            => (await _biobankReadService.ListBiobankFundersAsync(biobankId))
                .Select(bbFunder => new FunderModel
                {
                    FunderId = bbFunder.Id,
                    Name = bbFunder.Value
                }).ToList();

        public async Task<JsonResult> GetFundersAjax(int biobankId, int timeStamp = 0)
            //timeStamp can be used to avoid caching issues, notably on IE
            => Json(await GetFundersAsync(biobankId), JsonRequestBehavior.AllowGet);

        public ActionResult AddFunderSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"{name} has been successfully added to your list of funders!",
                FeedbackMessageType.Success);

            return RedirectToAction("Funders");
        }

        public async Task<ActionResult> AddFunderAjax(int biobankId)
        {
            var bb = await _organisationService.Get(biobankId);

            return PartialView("_ModalAddFunder", new AddFunderModel
            {
                BiobankName = bb.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddFunderAjax(AddFunderModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .Where(x => x.Errors.Count > 0)
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()
                });

            var funder = await _funderService.Get(model.FunderName);

            // Funder Doesn't Exist
            if (funder == null)
            {
                var useFreeText = Config.Get(ConfigKey.FundersFreeText, "false") == "true";

                if (useFreeText)
                {
                    // Add Funder to Database
                    funder = await _funderService.Add(new Funder
                    {
                        Value = model.FunderName
                    });
                }
                else
                {
                    ModelState.AddModelError("", "We couldn't find any funders with the name you entered.");

                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .Where(x => x.Errors.Count > 0)
                            .SelectMany(x => x.Errors)
                            .Select(x => x.ErrorMessage).ToList()
                    });
                }
            }

            //Add the funder/biobank relationship
            await _organisationService.AddFunder(
                funder.Id, SessionHelper.GetBiobankId(Session));

            //return success, and enough details for adding to the viewmodel's list
            return Json(new
            {
                success = true,
                funderId = funder.Id,
                name = model.FunderName
            });
        }

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> DeleteFunder(int funderId, string funderName)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            //remove them from the network
            await _organisationService.RemoveFunder(funderId, biobankId);

            SetTemporaryFeedbackMessage($"{funderName} has been removed from your list of funders!", FeedbackMessageType.Success);

            return RedirectToAction("Funders");
        }

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<JsonResult> SearchFunders(string wildcard)
        {
            var funders = await _funderService.List(wildcard);

            var funderResults = funders
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Value
                }).ToList();

            return Json(funderResults, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Collections
        [HttpGet]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> Collections()
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            var collections = await _collectionService.List(biobankId);

            // Build ViewModel.
            var model = new BiobankCollectionsModel
            {
                BiobankCollectionModels = collections.Select(x => new BiobankCollectionModel
                {
                    Id = x.CollectionId,
                    OntologyTerm = x.OntologyTerm.Value,
                    Title = x.Title,
                    StartYear = x.StartDate.Year,
                    MaterialTypes = Join(", ", _biobankReadService.ExtractDistinctMaterialTypes(x).Select(y => y)),
                    NumberOfSampleSets = x.SampleSets.Count
                })
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> AddCollection()
        {
            return View((AddCollectionModel)(await PopulateAbstractCRUDCollectionModel(new AddCollectionModel { FromApi = false })));
        }

        [HttpGet]
        public async Task<ActionResult> GetAssociatedDataTypeViewsAjax(string id)
        {
            AddCollectionModel model = new AddCollectionModel { FromApi = false };

            var timeFrames = (await _associatedDataProcurementTimeframeService.List())
               .Select(x => new AssociatedDataTimeFrameModel
               {
                   ProvisionTimeId = x.Id,
                   ProvisionTimeDescription = x.Value,
                   ProvisionTimeValue = x.DisplayValue
               });

            var types = (await _ontologyTermService.ListAssociatedDataTypesByOntologyTerm(id))
                     .Select(x => new AssociatedDataModel
                     {
                         DataTypeId = x.Id,
                         DataTypeDescription = x.Value,
                         DataGroupId = x.AssociatedDataTypeGroupId,
                         Message = x.Message,
                         TimeFrames = timeFrames,
                         isLinked = true
        });
            model.Groups = new List<AssociatedDataGroupModel>();
            var groups = await _associatedDataTypeGroupService.List();

            foreach (var g in groups)
            {
                var groupModel = new AssociatedDataGroupModel();
                groupModel.GroupId = g.Id;
                groupModel.Name = g.Value;
                groupModel.Types = types.Where(y => y.DataGroupId == g.Id).ToList();
                
                model.Groups.Add(groupModel);
            }

            //Check if types are valid
            foreach (var type in types)
            {
                type.Active = model.AssociatedDataModelsValid();
            }

            return PartialView("_LinkedAssociatedData", model);

        }

        private async Task<Boolean> IsLinkedAssociatedDataValid(
            List<AssociatedDataModel> linkedData, string ontologyTermId)
        {
            var associatedDataList = await _associatedDataTypeService.List();
            var newAssociatedData = associatedDataList.Where(x => linkedData.Find(y=>y.DataTypeId == x.Id) != null);
            // first check that all the data is present in the data list
            if(newAssociatedData.Count() != linkedData.Count())
            {
                return false;
            }
            // then check that all the linked data is linked to the ontologyTerm
            foreach (var type in newAssociatedData)
            {
                // only check linked data
                if (type.OntologyTerms != null && (type.OntologyTerms.Find(x => x.Id == ontologyTermId) == null))
                    {
                        return false;
                    }
            }
            
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCollection(AddCollectionModel model)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            // check linked types are valid
            List<AssociatedDataModel> linkedData = new List<AssociatedDataModel>();
            foreach (var group in model.Groups)
            {
                foreach (var type in group.Types)
                {
                    // create list of associated data types from model
                    linkedData.Add(type);
                }
            }
            // check that any linked associated data is related to the ontology term
            bool linkedIsValid = await IsLinkedAssociatedDataValid(linkedData, (await _ontologyTermService.Get(value: model.Diagnosis)).Id);


            if (await model.IsValid(ModelState, _ontologyTermService) && linkedIsValid)
            {
                


                var associatedData = model.ListAssociatedDataModels()
                    .Where(x => x.Active)
                    .Select(y => new CollectionAssociatedData
                    {
                        AssociatedDataTypeId = y.DataTypeId,
                        AssociatedDataProcurementTimeframeId = y.ProvisionTimeId // GroupID
                    })
                    .ToList();

                var consentRestrictions = model.ConsentRestrictions
                    .Where(x => x.Active)
                    .Select(x => new ConsentRestriction
                    {
                        Id = x.ConsentRestrictionId
                    })
                    .ToList();

                var ontologyTerm = await _ontologyTermService.Get(value: model.Diagnosis);

                // Create and Add New Collection
                var collection = await _collectionService.Add(new Collection
                {
                    OrganisationId = biobankId,
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = new DateTime(year: model.StartDate.Value, month: 1, day: 1),
                    AssociatedData = associatedData,
                    AccessConditionId = model.AccessCondition,
                    CollectionTypeId = model.CollectionType,
                    CollectionStatusId = model.CollectionStatus,
                    ConsentRestrictions = consentRestrictions,
                    OntologyTermId = ontologyTerm.Id,
                    FromApi = model.FromApi,
                    Notes = model.Notes
                });

                SetTemporaryFeedbackMessage("Collection added!", FeedbackMessageType.Success);

                return RedirectToAction("Collection", new
                {
                    id = collection.CollectionId
                });
            }
            else
            {
                //Populate Groups
                model.Groups = null;
                await PopulateAbstractCRUDCollectionModel(model);
            }

            return View((AddCollectionModel)(await PopulateAbstractCRUDCollectionModel(model)));
        }

        [HttpGet]
        [AuthoriseToAdministerCollection]
        public async Task<ActionResult> CopyCollection(int id)
        {

            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");


            // Copy and Add New Collection  
            var newCollection = await _collectionService.Copy(id, biobankId);

            var originalCollection = await _collectionService.Get(id);

            // Copy Sample Set 
            foreach (SampleSet sampleSet in originalCollection.SampleSets)
            {
                var newSampleSet = new SampleSet
                {
                    CollectionId = newCollection.CollectionId,
                    SexId = sampleSet.SexId,
                    AgeRangeId = sampleSet.AgeRangeId,
                    DonorCountId = sampleSet.DonorCountId,
                    MaterialDetails = sampleSet.MaterialDetails.Select(x =>
                       new MaterialDetail
                       {
                           MaterialTypeId = x.MaterialTypeId,
                           PreservationTypeId = x.PreservationTypeId,
                           StorageTemperatureId = x.StorageTemperatureId,
                           CollectionPercentageId = x.CollectionPercentageId,
                           MacroscopicAssessmentId = x.MacroscopicAssessmentId,
                           ExtractionProcedureId = x.ExtractionProcedureId
                       }
                      )
                      .ToList()
                };

                    // Add New SampleSet
                    await _biobankWriteService.AddSampleSetAsync(newSampleSet);
            }


            SetTemporaryFeedbackMessage("This is your copied collection. It has been saved and you are now free to edit it.", FeedbackMessageType.Success);

            return RedirectToAction("Collection", new
            {
                id = newCollection.CollectionId
            });
        }

        [HttpGet]
        [AuthoriseToAdministerCollection]
        public async Task<ViewResult> EditCollection(int id)
        {
            var collection = await _collectionService.Get(id);
            var consentRestrictions = await _consentRestrictionService.List();

            var groups = await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());

            var model = new EditCollectionModel
            {
                Id = collection.CollectionId,
                Diagnosis = collection.OntologyTerm.Value,
                Title = collection.Title,
                Description = collection.Description,
                StartDate = collection.StartDate.Year,
                AccessCondition = collection.AccessCondition.Id,
                FromApi = collection.FromApi,
                CollectionType = collection.CollectionType?.Id,
                CollectionStatus = collection.CollectionStatus.Id,
                Groups = groups.Groups

            };

            var assdatlist = model.ListAssociatedDataModels();

            foreach (var associatedDataModel in assdatlist)
            {
                var associatedData = collection.AssociatedData.FirstOrDefault(x => x.AssociatedDataTypeId == associatedDataModel.DataTypeId);

                if (associatedData != null)
                {
                    associatedDataModel.Active = true;
                    associatedDataModel.ProvisionTimeId = associatedData.AssociatedDataProcurementTimeframeId;
                }
            }

            return View((EditCollectionModel)(await PopulateAbstractCRUDCollectionModel(model, collection.ConsentRestrictions)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerCollection]
        public async Task<ActionResult> EditCollection(EditCollectionModel model)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            //Retrieve collection
            var collection = await _collectionService.Get(model.Id);

            if (collection.FromApi)
            {
                // Update description
                collection.Description = model.Description;

                await _collectionService.Update(collection);

                SetTemporaryFeedbackMessage("Collection updated!", FeedbackMessageType.Success);

                return RedirectToAction("Collection", new { id = model.Id });
            }

            if (await model.IsValid(ModelState, _ontologyTermService) && model.FromApi == false)
            {
                var associatedData = model.ListAssociatedDataModels()
                    .Where(x => x.Active)
                    .Select(y => new CollectionAssociatedData
                    {
                        CollectionId = model.Id,
                        AssociatedDataTypeId = y.DataTypeId,
                        AssociatedDataProcurementTimeframeId = y.ProvisionTimeId
                    }).ToList();

                var consentRestrictions = model.ConsentRestrictions
                    .Where(x => x.Active)
                    .Select(x => new ConsentRestriction
                    {
                        Id = x.ConsentRestrictionId
                    })
                    .ToList();

                var ontologyTerm = await _ontologyTermService.Get(value: model.Diagnosis);

                await _collectionService.Update(new Collection
                {
                    AccessConditionId = model.AccessCondition,
                    AssociatedData = associatedData,
                    CollectionId = model.Id,
                    CollectionStatusId = model.CollectionStatus,
                    CollectionTypeId = model.CollectionType,
                    ConsentRestrictions = consentRestrictions,
                    Description = model.Description,
                    FromApi = model.FromApi,
                    OntologyTermId = ontologyTerm.Id,
                    OrganisationId = biobankId,
                    StartDate = new DateTime(year: model.StartDate.Value, month: 1, day: 1),
                    Title = model.Title

                });

                SetTemporaryFeedbackMessage("Collection updated!", FeedbackMessageType.Success);

                return RedirectToAction("Collection", new { id = model.Id });
            }
            else
            {
                //Populate Groups
                model.Groups = null;
                await PopulateAbstractCRUDCollectionModel(model);
            }

            return View((EditCollectionModel)(await PopulateAbstractCRUDCollectionModel(model)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerCollection]
        public async Task<RedirectToRouteResult> DeleteCollection(int id)
        {
            if (!await _collectionService.IsFromApi(id) && await _collectionService.Delete(id))
            {
                SetTemporaryFeedbackMessage("Collection deleted!", FeedbackMessageType.Success);
                return RedirectToAction("Collections");
            }
            else
            {
                SetTemporaryFeedbackMessage(
                    "The system was unable to delete this collection. Please make sure it doesn't contain any Sample Sets before trying again.",
                    FeedbackMessageType.Danger);
                return RedirectToAction("Collection", new { id });
            }
        }

        [HttpGet]
        [AuthoriseToAdministerCollection]
        public async Task<ViewResult> Collection(int id)
        {
            var collection = await _collectionService.GetWithSampleSets(id);

            var model = new CollectionModel
            {
                Id = collection.CollectionId,
                Title = collection.Title,
                Description = collection.Description,
                OntologyTerm = collection.OntologyTerm.Value,
                StartDate = collection.StartDate,
                AccessCondition = collection.AccessCondition.Value,
                CollectionType = collection.CollectionType?.Value,
                FromApi = collection.FromApi,
                Notes = collection.Notes,
                AssociatedData = collection.AssociatedData.Select(x => new AssociatedDataSummaryModel
                {
                    Description = x.AssociatedDataType.Value,
                    ProvisionTime = x.AssociatedDataProcurementTimeframe.Value,
                    ProvisionTimeSortValue = x.AssociatedDataProcurementTimeframe.SortOrder
                }),
                SampleSets = collection.SampleSets.Select(sampleSet => new CollectionSampleSetSummaryModel
                {
                    Id = sampleSet.Id,
                    Sex = sampleSet.Sex.Value,
                    Age = sampleSet.AgeRange.Value,
                    MaterialTypes = Join(" / ", sampleSet.MaterialDetails.Select(x => x.MaterialType.Value).Distinct()),
                    PreservationTypes = Join(" / ", sampleSet.MaterialDetails.Select(x => x.PreservationType?.Value).Distinct()),
                    StorageTemperatures = Join(" / ", sampleSet.MaterialDetails.Select(x => x.StorageTemperature.Value).Distinct()),
                    ExtractionProcedures = Join(" / ", sampleSet.MaterialDetails.Where(x => x.ExtractionProcedure?.DisplayOnDirectory == true)
                                           .Select(x => x.ExtractionProcedure?.Value).Distinct())
                })
            };

            return View(model);
        }

        [HttpGet]
        [AuthoriseToAdministerCollection]
        public async Task<ViewResult> AddSampleSet(int id)
        {
            ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(id);
            var model = new AddSampleSetModel
            {
                CollectionId = id
            };
            return View((AddSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerCollection]
        public async Task<ActionResult> AddSampleSet(int id, AddSampleSetModel model)
        {
            var apiCheck = await _collectionService.IsFromApi(id);

            ViewData["CollectionApiStatus"] = apiCheck;

            if (model.IsValid(ModelState) && apiCheck == false)
            {
                var sampleSet = new SampleSet
                {
                    CollectionId = id,
                    SexId = model.Sex,
                    AgeRangeId = model.AgeRange,
                    DonorCountId = model.DonorCountId,
                    MaterialDetails = model.MaterialPreservationDetails.Select(x =>
                        new MaterialDetail
                        {
                            MaterialTypeId = x.materialType,
                            PreservationTypeId = x.preservationType,
                            StorageTemperatureId = x.storageTemperature,
                            CollectionPercentageId = x.percentage,
                            MacroscopicAssessmentId = x.macroscopicAssessment,
                            ExtractionProcedureId = x.extractionProcedure
                        }
                    )
                    .ToList()
                };

                // Add New SampleSet
                await _biobankWriteService.AddSampleSetAsync(sampleSet);

                SetTemporaryFeedbackMessage("Sample Set added!", FeedbackMessageType.Success);

                return RedirectToAction("Collection", new { id });
            }

            return View((AddSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
        }

        [HttpGet]
        [AuthoriseToAdministerSampleSet]
        public async Task<ActionResult> CopySampleSet(int id)
        {
            var sampleSet = await _biobankReadService.GetSampleSetByIdAsync(id);
            ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);
            SiteMaps.Current.CurrentNode.ParentNode.RouteValues["id"] = sampleSet.CollectionId;

            //Build the model using all details of the existing sampleset, except id, which is stored in a separate property
            var model = new CopySampleSetModel
            {
                OriginalId = id,
                CollectionId = sampleSet.CollectionId,
                Sex = sampleSet.SexId,
                AgeRange = sampleSet.AgeRangeId,
                DonorCountId = sampleSet.DonorCountId,

                MaterialPreservationDetailsJson = JsonConvert.SerializeObject(sampleSet.MaterialDetails.Select(x => new MaterialDetailModel
                {
                    materialType = x.MaterialTypeId,
                    preservationType = x.PreservationTypeId,
                    storageTemperature = x.StorageTemperatureId,
                    percentage = x.CollectionPercentageId,
                    macroscopicAssessment = x.MacroscopicAssessmentId,
                    extractionProcedure = x.ExtractionProcedureId
                }))
            };

            return View((CopySampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
        }

        [HttpPost]
        [AuthoriseToAdministerSampleSet]
        public async Task<ActionResult> CopySampleSet(int id, CopySampleSetModel model)
        {
            if (await _collectionService.IsFromApi(model.CollectionId))
            {
                return RedirectToAction("SampleSet", new { id = model.OriginalId });
            }
            var addModel = _mapper.Map<AddSampleSetModel>(model);
            return await AddSampleSet(model.CollectionId, addModel);
        }

        [HttpGet]
        [AuthoriseToAdministerSampleSet]
        public async Task<ViewResult> EditSampleSet(int id)
        {
            var sampleSet = await _biobankReadService.GetSampleSetByIdAsync(id);

            SiteMaps.Current.CurrentNode.ParentNode.ParentNode.RouteValues["id"] = sampleSet.CollectionId;

            ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);

            var model = new EditSampleSetModel
            {
                Id = sampleSet.Id,
                CollectionId = sampleSet.CollectionId,
                Sex = sampleSet.SexId,
                AgeRange = sampleSet.AgeRangeId,
                DonorCountId = sampleSet.DonorCountId,

                MaterialPreservationDetailsJson = JsonConvert.SerializeObject(sampleSet.MaterialDetails.Select(x => new MaterialDetailModel
                {
                    id = x.Id,
                    materialType = x.MaterialTypeId,
                    preservationType = x.PreservationTypeId,
                    storageTemperature = x.StorageTemperatureId,
                    percentage = x.CollectionPercentageId,
                    macroscopicAssessment = x.MacroscopicAssessmentId,
                    extractionProcedure = x.ExtractionProcedureId
                }))
            };

            return View((EditSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerSampleSet]
        public async Task<ActionResult> EditSampleSet(int id, EditSampleSetModel model)
        {
            var apiCheck = await _collectionService.IsFromApi(model.CollectionId);
            ViewData["CollectionApiStatus"] = apiCheck;

            if (model.IsValid(ModelState) && !apiCheck)
            {
                var sampleSet = new SampleSet
                {
                    Id = id,
                    SexId = model.Sex,
                    AgeRangeId = model.AgeRange,
                    DonorCountId = model.DonorCountId,
                    MaterialDetails = model.MaterialPreservationDetails.Select(x =>
                        new MaterialDetail
                        {
                            Id = x.id ?? 0,
                            MaterialTypeId = x.materialType,
                            PreservationTypeId = x.preservationType,
                            StorageTemperatureId = x.storageTemperature,
                            CollectionPercentageId = x.percentage,
                            MacroscopicAssessmentId = x.macroscopicAssessment,
                            ExtractionProcedureId = x.extractionProcedure
                        }
                    )
                    .ToList()
                };

                // Update SampleSet
                await _biobankWriteService.UpdateSampleSetAsync(sampleSet);

                SetTemporaryFeedbackMessage("Sample Set updated!", FeedbackMessageType.Success);

                return RedirectToAction("SampleSet", new { id = model.Id });
            }

            SiteMaps.Current.CurrentNode.ParentNode.ParentNode.RouteValues["id"] = model.CollectionId;

            return View((EditSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerSampleSet]
        public async Task<RedirectToRouteResult> DeleteSampleSet(int id, int collectionId)
        {
            if (!await _collectionService.IsFromApi(collectionId))
            {
                await _biobankWriteService.DeleteSampleSetAsync(id);
                SetTemporaryFeedbackMessage("Sample Set deleted!", FeedbackMessageType.Success);
            }
            return RedirectToAction("Collection", new { id = collectionId });
        }

        [HttpGet]
        [AuthoriseToAdministerSampleSet]
        public async Task<ViewResult> SampleSet(int id)
        {
            var sampleSet = await _biobankReadService.GetSampleSetByIdAsync(id);
            var assessments = await _macroscopicAssessmentService.List();

            SiteMaps.Current.CurrentNode.ParentNode.RouteValues["id"] = sampleSet.CollectionId;

            ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);

            var model = new SampleSetModel
            {
                Id = sampleSet.Id,
                CollectionId = sampleSet.CollectionId,
                Sex = sampleSet.Sex.Value,
                AgeRange = sampleSet.AgeRange.Value,
                DonorCount = sampleSet.DonorCount.Value,
                MaterialPreservationDetails = sampleSet.MaterialDetails.Select(x => new MaterialPreservationDetailModel
                {
                    CollectionPercentage = x.CollectionPercentage?.Value,
                    MacroscopicAssessment = x.MacroscopicAssessment.Value,
                    MaterialType = x.MaterialType.Value,
                    PreservationType = x.PreservationType?.Value,
                    StorageTemperature = x.StorageTemperature.Value,
                    ExtractionProcedure = x.ExtractionProcedure?.DisplayOnDirectory == true ? x.ExtractionProcedure.Value : null

                }),
                ShowMacroscopicAssessment = (assessments.Count() > 1)
            };

            return View(model);
        }

        #region View Model Populators
        private async Task<AbstractCRUDCollectionModel> PopulateAbstractCRUDCollectionModel(
            AbstractCRUDCollectionModel model,
            IEnumerable<ConsentRestriction> consentRestrictions = null)
        {

            model.AccessConditions = (await _accessConditionService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                })
                .OrderBy(x => x.SortOrder);

            model.CollectionTypes = (await _collectionTypeService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                })
                .OrderBy(x => x.SortOrder);

            model.CollectionStatuses = (await _collectionStatusService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                })
                .OrderBy(x => x.SortOrder);

            model.ConsentRestrictions = (await _consentRestrictionService.List())
                .OrderBy(x => x.SortOrder)
                .Select(x => new Models.Biobank.ConsentRestrictionModel
                {
                    ConsentRestrictionId = x.Id,
                    Description = x.Value,
                    Active = consentRestrictions != null && consentRestrictions.Any(y => y.Id == x.Id)
                });

            //if not null keeps previous groups values
            if (model.Groups == null)
            {
                var groups = await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());
                model.Groups = groups.Groups;
            }


            return model;
        }


         private async Task<AbstractCRUDCapabilityModel> PopulateAbstractCRUDAssociatedData(

            AbstractCRUDCapabilityModel model, Boolean excludeLinkedData = false)
        {
            var timeFrames = (await _associatedDataProcurementTimeframeService.List())
                .Select(x => new AssociatedDataTimeFrameModel
                {
                    ProvisionTimeId = x.Id,
                    ProvisionTimeDescription = x.Value,
                    ProvisionTimeValue = x.DisplayValue
                });
            var typeList = await _associatedDataTypeService.List();
            if (excludeLinkedData)
            {
                typeList = typeList.Where(x => x.OntologyTerms == null).ToList();
            }

            else
            {
                var ontologyTerm = await _ontologyTermService.Get(value: model.Diagnosis);
                if(ontologyTerm != null) {
                    typeList = typeList.Where(x => x.OntologyTerms == null || (x.OntologyTerms.Find(y => y.Id == ontologyTerm.Id) != null)).ToList();
                }   
            }

            var types = typeList
                     .Select(x => new AssociatedDataModel
                     {
                         DataTypeId = x.Id,
                         DataTypeDescription = x.Value,
                         DataGroupId = x.AssociatedDataTypeGroupId,
                         Message = x.Message,
                         TimeFrames = timeFrames,
                         isLinked = x.OntologyTerms != null
                     });
            
            model.Groups = new List<AssociatedDataGroupModel>();
            var groups = await _associatedDataTypeGroupService.List();
            foreach (var g in groups)
            {
                var groupModel = new AssociatedDataGroupModel();
                groupModel.GroupId = g.Id;
                groupModel.Name = g.Value;
                groupModel.Types = types.Where(y => y.DataGroupId == g.Id).ToList();
                model.Groups.Add(groupModel);
            }
            
            //Check if types are valid
            foreach (var type in types)
            {
                type.Active = model.AssociatedDataModelsValid();
            }

            return model;
        }

        private async Task<AbstractCRUDSampleSetModel> PopulateAbstractCRUDSampleSetModel(AbstractCRUDSampleSetModel model)
        {
            model.Sexes = (await _sexService.List())
                .Select(
                    x => new ReferenceDataModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder
                    })
                .OrderBy(x => x.SortOrder);

            model.AgeRanges = (await _ageRangeService.List())
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            model.DonorCounts = (await _donorCountService.List())
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            model.MaterialTypes = (await _materialTypeService.List())
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            model.ExtractionProcedures = (await _ontologyTermService.List(tags: new List<string>
                {
                    SnomedTags.ExtractionProcedure
                }, onlyDisplayable: true))
                .Select(
                    x =>
                        new OntologyTermModel
                        {
                            OntologyTermId = x.Id,
                            Description = x.Value,
                        })
                .OrderBy(x => x.Description);

            model.PreservationTypes = (await _preservationTypeService.List())
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            model.StorageTemperatures = (await _storageTemperatureService.List())
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            model.Percentages = (await _collectionPercentageService.List())
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            var assessments = await _macroscopicAssessmentService.List();

            model.MacroscopicAssessments = assessments
                .Select(
                    x =>
                        new ReferenceDataModel
                        {
                            Id = x.Id,
                            Description = x.Value,
                            SortOrder = x.SortOrder
                        })
                .OrderBy(x => x.SortOrder);

            model.ShowMacroscopicAssessment = (assessments.Count() > 1);

            if (model.DonorCountId > 0)
            {
                var donorCountList = model.DonorCounts.ToList();
                model.DonorCountSliderPosition = donorCountList.IndexOf(donorCountList.First(x => x.Id == model.DonorCountId));
            }

            return model;
        }
        #endregion
        #endregion

        #region Capabilities
        [HttpGet]
        public async Task<ActionResult> Capabilities()
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            // Call service to get capabilities for logged in BioBank.
            var capabilities = (await _biobankReadService.ListCapabilitiesAsync(biobankId)).ToList();

            // Build ViewModel.
            var model = new BiobankCapabilitiesModel
            {
                BiobankCapabilityModels = capabilities.Select(x => new BiobankCapabilityModel
                {
                    Id = x.DiagnosisCapabilityId,
                    OntologyTerm = x.OntologyTerm.Value,
                    Protocol = x.SampleCollectionMode.Value
                })
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> AddCapability()
        {
            return View((AddCapabilityModel)(await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel())));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCapability(AddCapabilityModel model)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            if (await model.IsValid(ModelState, _ontologyTermService))
            {
                var associatedData = model.ListAssociatedDataModels()
                    .Where(x => x.Active)
                    .Select(y => new CapabilityAssociatedData
                    {
                        AssociatedDataTypeId = y.DataTypeId,
                        AssociatedDataProcurementTimeframeId = y.ProvisionTimeId
                    });

                await _biobankWriteService.AddCapabilityAsync(new CapabilityDTO
                {
                    OrganisationId = biobankId,
                    OntologyTerm = model.Diagnosis,
                    BespokeConsentForm = model.BespokeConsentForm,
                    BespokeSOP = model.BespokeSOP,
                    AnnualDonorExpectation = model.AnnualDonorExpectation.Value
                },
                associatedData);

                SetTemporaryFeedbackMessage("Capability added!", FeedbackMessageType.Success);

                return RedirectToAction("Capabilities");
            }

            return View((AddCapabilityModel)(await PopulateAbstractCRUDAssociatedData(model)));
        }

        [HttpGet]
        [AuthoriseToAdministerCapability]
        public async Task<ViewResult> EditCapability(int id)
        {
            var capability = await _biobankReadService.GetCapabilityByIdAsync(id);
            var groups = await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());

            var model = new EditCapabilityModel
            {
                Id = id,
                Diagnosis = capability.OntologyTerm.Value,
                AnnualDonorExpectation = capability.AnnualDonorExpectation,
                Groups = groups.Groups
            };

            switch (capability.SampleCollectionModeId)
            {
                case 1:
                    model.BespokeConsentForm = true;
                    model.BespokeSOP = false;
                    break;
                case 2:
                    model.BespokeConsentForm = false;
                    model.BespokeSOP = true;
                    break;
                case 3:
                    model.BespokeConsentForm = true;
                    model.BespokeSOP = true;
                    break;
                default:
                    model.BespokeConsentForm = false;
                    model.BespokeSOP = false;
                    break;
            }

            foreach (var associatedDataModel in model.ListAssociatedDataModels())
            {
                var associatedData = capability.AssociatedData.FirstOrDefault(x => x.AssociatedDataTypeId == associatedDataModel.DataTypeId);

                if (associatedData != null)
                {
                    associatedDataModel.Active = true;
                    associatedDataModel.ProvisionTimeId = associatedData.AssociatedDataProcurementTimeframeId;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerCapability]
        public async Task<ActionResult> EditCapability(EditCapabilityModel model)
        {
            if (await model.IsValid(ModelState, _ontologyTermService))
            {
                var associatedData = model.ListAssociatedDataModels()
                    .Where(x => x.Active)
                    .Select(y => new CapabilityAssociatedData
                    {
                        DiagnosisCapabilityId = model.Id,
                        AssociatedDataTypeId = y.DataTypeId,
                        AssociatedDataProcurementTimeframeId = y.ProvisionTimeId
                    }).ToList();

                await _biobankWriteService.UpdateCapabilityAsync(new CapabilityDTO
                {
                    Id = model.Id,
                    OntologyTerm = model.Diagnosis,
                    BespokeConsentForm = model.BespokeConsentForm,
                    BespokeSOP = model.BespokeSOP,
                    AnnualDonorExpectation = model.AnnualDonorExpectation.Value
                },
                associatedData);

                SetTemporaryFeedbackMessage("Capability updated!", FeedbackMessageType.Success);

                return RedirectToAction("Capability", new { id = model.Id });
            }

            return View((EditCapabilityModel)(await PopulateAbstractCRUDAssociatedData(model)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthoriseToAdministerCapability]
        public async Task<RedirectToRouteResult> DeleteCapability(int id)
        {
            await _biobankWriteService.DeleteCapabilityAsync(id);

            SetTemporaryFeedbackMessage("Capability deleted!", FeedbackMessageType.Success);

            return RedirectToAction("Capabilities");
        }

        [HttpGet]
        [AuthoriseToAdministerCapability]
        public async Task<ViewResult> Capability(int id)
        {
            var capability = await _biobankReadService.GetCapabilityByIdAsync(id);

            var model = new CapabilityModel
            {
                Id = capability.DiagnosisCapabilityId,
                OntologyTerm = capability.OntologyTerm.Value,
                Protocols = capability.SampleCollectionMode.Value,
                AnnualDonorExpectation = capability.AnnualDonorExpectation,
                AssociatedData = capability.AssociatedData.Select(x => new AssociatedDataSummaryModel
                {
                    Description = x.AssociatedDataType.Value,
                    ProvisionTime = x.AssociatedDataProcurementTimeframe.Value,
                    ProvisionTimeSortValue = x.AssociatedDataProcurementTimeframe.SortOrder
                })
            };

            return View(model);
        }

        #endregion

        #region Network Acceptance

        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> NetworkAcceptance()
        {
            var biobankId = SessionHelper.GetBiobankId(Session);
            var organisationNetworks = await _networkService.ListOrganisationNetworks(biobankId);
            var networkList = new List<NetworkAcceptanceModel>();
            foreach (var orgNetwork in organisationNetworks)
            {
                var network = await _networkService.Get(orgNetwork.NetworkId);
                var organisation = new NetworkAcceptanceModel
                {
                    BiobankId = biobankId,
                    NetworkId = network.NetworkId,
                    NetworkName = network.Name,
                    NetworkDescription = network.Description,
                    NetworkEmail = network.Email,
                    ApprovedDate = orgNetwork.ApprovedDate
                };
                networkList.Add(organisation);

            }

            var model = new AcceptanceModel
            {
                NetworkRequests = networkList
            };

            return View(model);
        }

        public async Task<ActionResult> AcceptNetworkRequest(int biobankId, int networkId)
        {
            var organisationNetwork = await _networkService.GetOrganisationNetwork(biobankId, networkId);

            organisationNetwork.ApprovedDate = DateTime.Now;
            await _networkService.UpdateOrganisationNetwork(organisationNetwork);

            SetTemporaryFeedbackMessage("Biobank added to the network successfully", FeedbackMessageType.Success);

            return RedirectToAction("NetworkAcceptance");
        }
        #endregion

        #region Publications 
        [HttpGet]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> Publications()
        {
            //If turned off in site config
            if (await _configService.GetFlagConfigValue(ConfigKey.DisplayPublications) == false)
                return HttpNotFound();

            return View(SessionHelper.GetBiobankId(Session));
        }


        [HttpGet]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<JsonResult> GetPublicationsAjax()
        {
            //If turned off in site config
            if (await _configService.GetFlagConfigValue(ConfigKey.DisplayPublications) == false)
                return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);

            var biobankId = SessionHelper.GetBiobankId(Session);
            var biobankPublications = Enumerable.Empty<BiobankPublicationModel>();

            if (biobankId != 0)
            {
                var publications = await _publicationService.ListByOrganisation(biobankId);

                biobankPublications = _mapper.Map<List<BiobankPublicationModel>>(publications);
            }

            return new JsonResult
            {
                Data = biobankPublications,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,

                // Handle biobanks with large amount of publications
                // Ideally switch to server-side processing with multiple GET requests
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<Publication> PublicationSearch(string publicationId, int biobankId)
        {
            // Find Given Publication
            var publications = await _publicationService.ListByOrganisation(biobankId);
            var publication = publications.Where(x => x.PublicationId == publicationId).FirstOrDefault();

            //retrieve from EPMC if not found
            if (publication == null)
            {
                try
                {
                    using var client = new HttpClient();
                    var buildUrl = new UriBuilder(ConfigurationManager.AppSettings["EpmcApiUrl"])
                    {
                        Query = $"query=ext_id:{publicationId} AND SRC:MED" +
                                $"&cursorMark=*" +
                                $"&resultType=lite" +
                                $"&format=json"
                    };
                    buildUrl.Path += "webservices/rest/search";
                    var response = await client.GetStringAsync(buildUrl.Uri);

                    var jPublications = JObject.Parse(response).SelectToken("resultList.result");
                    return _mapper.Map<Publication>(jPublications?.ToObject<List<PublicationSearchModel>>().FirstOrDefault());
                }
                catch (Exception e) when (
                    e is HttpRequestException ||
                    e is JsonReaderException ||
                    e is UriFormatException)
                {
                    // Log Error via Application Insights
                    var ai = new TelemetryClient();
                    ai.TrackException(e);

                    return null;
                }

            }

            return publication;
        }

        [HttpGet]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<JsonResult> RetrievePublicationsAjax(string publicationId)
        {
            //TODO: Merge with core for when we rewrite the directory in core.
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0 || IsNullOrEmpty(publicationId))
                return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
            else
            {
                // Find Publication locally
                var publications = await _publicationService.ListByOrganisation(biobankId);
                var publication = publications.Where(x => x.PublicationId == publicationId).FirstOrDefault();

                // search online
                if (publication == null)
                    publication = await PublicationSearch(publicationId, biobankId);

                return publication != null
                    ? Json(_mapper.Map<BiobankPublicationModel>(publication), JsonRequestBehavior.AllowGet)
                    : Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<JsonResult> AddPublicationAjax(string publicationId)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0 || IsNullOrEmpty(publicationId))
                return Json(new EmptyResult());

            // Try Accept Local Publication
            var publication = await _publicationService.Claim(publicationId, biobankId);

            // No Local Publication - Fetch
            if (publication == null)
            {
                publication = await PublicationSearch(publicationId, biobankId);

                // No Publication Found
                if (publication == null)
                    return Json(new EmptyResult());

                // Add Publication to DB
                publication.Accepted = true;
                publication.OrganisationId = biobankId;

                publication = await _publicationService.Create(publication);
            }

            return Json(_mapper.Map<BiobankPublicationModel>(publication));
        }

        [HttpPost]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<JsonResult> ClaimPublicationAjax(string publicationId, bool accept)
        {
            var biobankId = SessionHelper.GetBiobankId(Session);

            if (biobankId == 0 || IsNullOrEmpty(publicationId))
                return Json(new EmptyResult());

            // Update Publication
            var publication = await _publicationService.Claim(publicationId, biobankId, accept);

            return Json(_mapper.Map<BiobankPublicationModel>(publication), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddPublicationSuccessFeedback(string publicationId)
        {
            SetTemporaryFeedbackMessage($"The publication with PubMed ID \"{publicationId}\" has been added successfully.", FeedbackMessageType.Success);
            return Redirect("Publications");
        }
        #endregion

        #region Annual Stats

        [HttpGet]
        public async Task<ActionResult> AnnualStats()
        => View(new BiobankAnnualStatsModel
        {
            AnnualStatisticGroups = await _annualStatisticGroupService.List(),
            BiobankAnnualStatistics = (await _organisationService.Get(SessionHelper.GetBiobankId(Session))).OrganisationAnnualStatistics
        });

        [HttpPost]
        public async Task<JsonResult> UpdateAnnualStatAjax(AnnualStatModel model)
        {
            if (model.Year > DateTime.Now.Year)
                ModelState.AddModelError("", $"Year value for annual stat cannot be in the future.");

            var annualStatsStartYear = int.Parse(ConfigurationManager.AppSettings["AnnualStatsStartYear"]);
            if (model.Year < annualStatsStartYear)
                ModelState.AddModelError("", $"Year value for annual stat cannot be earlier than {annualStatsStartYear}");

            if (!(model.Value is null) && model.Value < 0)
                ModelState.AddModelError("", $"Annual stat value cannot be less than 0.");

            // if there are any errors, return false
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .Where(x => x.Errors.Count > 0)
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()
                });

            // no validation errors at this point, so proceed
            await _biobankWriteService.UpdateOrganisationAnnualStatisticAsync(
                SessionHelper.GetBiobankId(Session),
                model.AnnualStatisticId,
                model.Value,
                model.Year);

            return Json(new
            {
                success = true
            });
        }

        #endregion

        #region Analytics
        public async Task<ActionResult> Analytics(int year = 0, int endQuarter = 0, int reportPeriod = 0)
        {
            //If turned off in site config
            if (await _configService.GetFlagConfigValue(ConfigKey.DisplayAnalytics) == false)
                return HttpNotFound();

            var biobankId = SessionHelper.GetBiobankId(Session);

            //set default options
            if (biobankId == 0)
                return RedirectToAction("Index", "Home");
            if (year == 0)
                year = DateTime.Today.Year;
            if (endQuarter == 0)
                endQuarter = ((DateTime.Today.Month + 2) / 3);
            if (reportPeriod == 0)
                reportPeriod = 5;

            try
            {
                var model = _mapper.Map<BiobankAnalyticReport>(await _analyticsReportGenerator.GetBiobankReport(biobankId, year, endQuarter, reportPeriod));
                return View(model);
            }
            catch (Exception e)
            {
                var message = e switch
                {
                    JsonSerializationException _ => "The API Response Body could not be processed.",
                    KeyNotFoundException _ => "Couldn't find the specified Biobank.",
                    HttpRequestException _ => "The API Request failed.",
                    _ => "An unknown error occurred and has been logged."
                };

                var outer = new Exception(message, e);

                // Log Error via Application Insights
                var ai = new TelemetryClient();
                ai.TrackException(outer);

                ModelState.AddModelError(Empty, outer);
                return View(new BiobankAnalyticReport());
            }
        }
        #endregion

        #region Submissions

        [HttpGet]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> Submissions()
        {
            var model = new SubmissionsModel();

            //populate drop downs
            model.AccessConditions = (await _accessConditionService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                }).OrderBy(x => x.SortOrder);

            model.CollectionTypes = (await _collectionTypeService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                }).OrderBy(x => x.SortOrder);

            //get currently selected values from org (if applicable)
            var biobankId = SessionHelper.GetBiobankId(Session);
            var biobank = await _organisationService.GetForBulkSubmissions(biobankId);

            model.BiobankId = biobankId;
            model.AccessCondition = biobank.AccessConditionId;
            model.CollectionType = biobank.CollectionTypeId;
            model.ClientId = biobank.ApiClients.FirstOrDefault()?.ClientId;

            return View(model);
        }

        [HttpPost]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submissions(SubmissionsModel model)
        {
            //update Organisations table
            var biobankId = SessionHelper.GetBiobankId(Session);

            var biobank = await _organisationService.Get(biobankId);
            biobank.AccessConditionId = model.AccessCondition;
            biobank.CollectionTypeId = model.CollectionType;

            await _organisationService.Update(biobank);

            //Set feedback and redirect
            SetTemporaryFeedbackMessage("Submissions settings updated!", FeedbackMessageType.Success);

            return RedirectToAction("Submissions");
        }

        [HttpPost]
        [Authorize(ClaimType = CustomClaimType.Biobank)]
        public async Task<ActionResult> GenerateApiKeyAjax(int biobankId)
        {
            var credentials =
                await _organisationService.IsApiClient(biobankId)
                    ? await _organisationService.GenerateNewSecretForBiobank(biobankId)
                    : await _organisationService.GenerateNewApiClient(biobankId);

            return Json(new
            {
                ClientId = credentials.Key,
                ClientSecret = credentials.Value
            });
        }
        #endregion

        public ActionResult Suspended(string biobankName)
        {
            var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
            ModelState.AddModelError("",
                $"{biobankName} has been suspended. " +
                $"Please contact ADAC via <a href=\"mailto:{supportEmail}\">{supportEmail}</a>");

            return View("GlobalErrors");
        }
    }
}
