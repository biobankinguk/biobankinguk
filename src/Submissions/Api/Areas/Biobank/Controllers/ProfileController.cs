using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Data.Transforms.Url;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Services;
using Biobanks.Submissions.Api.Areas.Admin.Models;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Extensions;
using Biobanks.Submissions.Api.Models.Directory;
using Biobanks.Submissions.Api.Models.Profile;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;

[Area("Biobank")]
public class ProfileController : Controller
{
    private readonly AnnualStatisticGroupService _annualStatisticGroupService;
    private readonly BiobankService _biobankService;
    private readonly BiobankWriteService _biobankWriteService;
    private readonly ConfigService _configService;
    private readonly CountyService _countyService;
    private readonly CountryService _countryService;
    private readonly FunderService _funderService;
    private readonly Mapper _mapper;
    private readonly NetworkService _networkService;
    private readonly OrganisationDirectoryService _organisationService;
    private readonly PublicationService _publicationService;
    private readonly RegistrationReasonService _registrationReasonService;
    private readonly ServiceOfferingService _serviceOfferingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(
        AnnualStatisticGroupService annualStatisticGroupService,
        BiobankService biobankService,
        BiobankWriteService biobankWriteService,
        ConfigService configService,
        CountyService countyService,
        CountryService countryService,
        FunderService funderService,
        Mapper mapper,
        NetworkService networkService,
        OrganisationDirectoryService organisationService,
        PublicationService publicationService,
        RegistrationReasonService registrationReasonService,
        ServiceOfferingService serviceOfferingService,
        UserManager<ApplicationUser> userManager)
    {
        _annualStatisticGroupService = annualStatisticGroupService;
        _biobankService = biobankService;
        _biobankWriteService = biobankWriteService;
        _configService = configService;
        _countyService = countyService;
        _countryService = countryService;
        _funderService = funderService;
        _mapper = mapper;
        _networkService = networkService;
        _organisationService = organisationService;
        _publicationService = publicationService;
        _registrationReasonService = registrationReasonService;
        _serviceOfferingService = serviceOfferingService;
        _userManager = userManager;
    }
    
    #region Details

    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> Index(int biobankId)
    {
        var model = await GetBiobankDetailsModelAsync(biobankId);

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
        await _organisationService.AddUserToOrganisation(_userManager.GetUserId(User), biobank.OrganisationId);
        
        //update the request to show org created
        var request = await _organisationService.GetRegistrationRequestByEmail(User.Identity.Name);
        request.OrganisationCreatedDate = DateTime.Now;
        request.OrganisationExternalId = biobank.OrganisationExternalId;
        await _organisationService.UpdateRegistrationRequest(request);

        //add a claim now that they're associated with the biobank
        _userManager.AddClaimsAsync(await _userManager.GetUserAsync(User),new List<Claim>
                {
                    new Claim(CustomClaimType.Biobank, JsonConvert.SerializeObject(new KeyValuePair<int, string>(biobank.OrganisationId, biobank.Name)))
                });

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

    public async Task<ActionResult> Edit(int biobankId, bool detailsIncomplete = false)
    {
        var sampleResource = await _configService.GetSiteConfigValue(ConfigKey.SampleResourceName);

        if (detailsIncomplete)
            this.SetTemporaryFeedbackMessage("Please fill in the details below for your " + sampleResource + ". Once you have completed these, you'll be able to perform other administration tasks",
                FeedbackMessageType.Info);

        return View(await GetBiobankDetailsModelAsync(biobankId)); 
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
                    ModelState.AddModelError(string.Empty, $"{model.Url} does not appear to be a valid URL.");
                    model = await AddCountiesToModel(model);
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, $"Could not access URL {model.Url}.");
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
        this.SetTemporaryFeedbackMessage(sampleResource + " details updated!", FeedbackMessageType.Success);

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

    private async Task<BiobankDetailsModel> NewBiobankDetailsModelAsync(int biobankId)
    {
        //the biobank doesn't exist yet, but a request should, so we can get the name
        var request = await _organisationService.GetRegistrationRequest(biobankId);

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

    private async Task<BiobankDetailsModel> GetBiobankDetailsModelAsync(int biobankId)
    {
        //having a biobankId claim means we can definitely get a biobank for that claim and return a model for that
        var bb = await _organisationService.Get(biobankId);

        //Try and get any service offerings for this biobank
        var bbServices =
                await _biobankService.ListBiobankServiceOfferingsAsync(bb.OrganisationId);

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
        if (!HttpContext.Request.Form.Files.Any())
            return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

        var formFile = HttpContext.Request.Form.Files["TempLogo"];

        if (formFile == null)
            return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

        if (formFile.Length > 1000000)
            return Json(new KeyValuePair<bool, string>(false, "The file you supplied is too large. Logo image files must be 1Mb or less."));
        
         try
        {
            if (formFile.ValidateAsLogo())
            {
                var logoStream = formFile.ToProcessableStream();
                Session[TempBiobankLogoSessionId] =
                    ImageService.ResizeImageStream(logoStream, maxX: 300, maxY: 300)
                    .ToArray();
                Session[TempBiobankLogoContentTypeSessionId] = fileBaseWrapper.ContentType;

                return
                    Json(new KeyValuePair<bool, string>(true,
                        Url.Action("TempLogo", "Profile")));
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

    // TODO: Replace session (this method is not used, so could alternatively be removed)
    // [HttpPost]
    // [Authorize(Roles = "BiobankAdmin")]
    // public void RemoveTempLogo()
    // {
    //     Session[TempBiobankLogoSessionId] = null;
    //     Session[TempBiobankLogoContentTypeSessionId] = null;
    // }
    
    #endregion

    #region Funders

    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> Funders(int biobankId)
    {
        if (biobankId == 0)
            return RedirectToAction("Index", "Home");
    
        return View(new BiobankFundersModel()
        {
            BiobankId = biobankId,
            Funders = await GetFundersAsync(biobankId)
        });
    }
    
    private async Task<List<FunderModel>> GetFundersAsync(int biobankId)
        => (await _biobankService.ListBiobankFundersAsync(biobankId))
            .Select(bbFunder => new FunderModel
            {
                FunderId = bbFunder.Id,
                Name = bbFunder.Value
            }).ToList();
    
    public async Task<JsonResult> GetFundersAjax(int biobankId, int timeStamp = 0)
        //timeStamp can be used to avoid caching issues, notably on IE
        => Json(await GetFundersAsync(biobankId));
    
    public ActionResult AddFunderSuccess(string name)
    {
        //This action solely exists so we can set a feedback message
    
        this.SetTemporaryFeedbackMessage($"{name} has been successfully added to your list of funders!",
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
    public async Task<ActionResult> AddFunderAjax(AddFunderModel model, int biobankId)
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
            var useFreeText = await _configService.GetSiteConfigValue(ConfigKey.FundersFreeText) == "true";

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
        await _organisationService.AddFunder(funder.Id, biobankId);
    
        //return success, and enough details for adding to the viewmodel's list
        return Ok(new
        {
            success = true,
            funderId = funder.Id,
            name = model.FunderName
        });
    }
    
    [Authorize( CustomClaimType.Biobank)]
    public async Task<ActionResult> DeleteFunder(int funderId, string funderName, int biobankId)
    {
        if (biobankId == 0)
            return RedirectToAction("Index", "Home");
    
        //remove them from the network
        await _organisationService.RemoveFunder(funderId, biobankId);
    
        this.SetTemporaryFeedbackMessage($"{funderName} has been removed from your list of funders!", FeedbackMessageType.Success);
    
        return RedirectToAction("Funders");
    }
    
    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> SearchFunders(string wildcard)
    {
        var funders = await _funderService.List(wildcard);
    
        var funderResults = funders
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Value
            }).ToList();
    
        return Ok(funderResults);
    }
    
    #endregion
    
    #region Publications
    
    [HttpGet]
    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> Publications(int biobankId)
    {
        //If turned off in site config
        if (await _configService.GetFlagConfigValue(ConfigKey.DisplayPublications) == false)
            return NotFound();

        return View(biobankId);
    }


    [HttpGet]
    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> GetPublicationsAjax(int biobankId)
    {
        //If turned off in site config
        if (await _configService.GetFlagConfigValue(ConfigKey.DisplayPublications) == false)
            return Json(new EmptyResult());
        
        var biobankPublications = Enumerable.Empty<BiobankPublicationModel>();

        if (biobankId != 0)
        {
            var publications = await _publicationService.ListByOrganisation(biobankId);

            biobankPublications = _mapper.Map<List<BiobankPublicationModel>>(publications);
        }

        return Ok(biobankPublications);

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
                var buildUrl = new UriBuilder(await _configService.GetSiteConfigValue(ConfigKey.EpmcApiUrl))
               
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
    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> RetrievePublicationsAjax(string publicationId, int biobankId)
    {
        
        if (biobankId == 0 || string.IsNullOrEmpty(publicationId))
            return Ok(new EmptyResult());
        else
        {
            // Find Publication locally
            var publications = await _publicationService.ListByOrganisation(biobankId);
            var publication = publications.Where(x => x.PublicationId == publicationId).FirstOrDefault();

            // search online
            if (publication == null)
                publication = await PublicationSearch(publicationId, biobankId);

            return publication != null
                ? Ok(_mapper.Map<BiobankPublicationModel>(publication))
                : Ok(new EmptyResult());
        }
    }

    [HttpPost]
    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> AddPublicationAjax(string publicationId, int biobankId)
    {
        if (biobankId == 0 || string.IsNullOrEmpty(publicationId))
            return Ok(new EmptyResult());

        // Try Accept Local Publication
        var publication = await _publicationService.Claim(publicationId, biobankId);

        // No Local Publication - Fetch
        if (publication == null)
        {
            publication = await PublicationSearch(publicationId, biobankId);

            // No Publication Found
            if (publication == null)
                return Ok(new EmptyResult());

            // Add Publication to DB
            publication.Accepted = true;
            publication.OrganisationId = biobankId;

            publication = await _publicationService.Create(publication);
        }

        return Ok(_mapper.Map<BiobankPublicationModel>(publication));
    }

    [HttpPost]
    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> ClaimPublicationAjax(string publicationId, bool accept, int biobankId)
    {
        if (biobankId == 0 || string.IsNullOrEmpty(publicationId))
            return Ok(new EmptyResult());

        // Update Publication
        var publication = await _publicationService.Claim(publicationId, biobankId, accept);

        return Ok(_mapper.Map<BiobankPublicationModel>(publication));
    }

    public ActionResult AddPublicationSuccessFeedback(string publicationId)
    {
        this.SetTemporaryFeedbackMessage($"The publication with PubMed ID \"{publicationId}\" has been added successfully.", FeedbackMessageType.Success);
        return Redirect("Publications");
    }
    
    #endregion
    
    
    
}
