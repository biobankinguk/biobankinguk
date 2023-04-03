using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Transforms.Url;
using Biobanks.Directory.Areas.Admin.Models.Funders;
using Biobanks.Directory.Areas.Biobank.Models.Profile;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Config;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Extensions;
using Biobanks.Directory.Filters;
using Biobanks.Directory.Models.Directory;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Submissions;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BiobankPublicationModel = Biobanks.Directory.Areas.Biobank.Models.Profile.BiobankPublicationModel;

namespace Biobanks.Directory.Areas.Biobank.Controllers;

[Area("Biobank")]
[Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
[SuspendedWarning]
public class ProfileController : Controller
{
    private readonly IReferenceDataCrudService<AnnualStatisticGroup> _annualStatisticGroupService;
    private readonly IBiobankService _biobankService;
    private readonly IConfigService _configService;
    private readonly IReferenceDataCrudService<County> _countyService;
    private readonly IReferenceDataCrudService<Country> _countryService;
    private readonly IReferenceDataCrudService<Funder> _funderService;
    private readonly IMapper _mapper;
    private readonly INetworkService _networkService;
    private readonly IOrganisationDirectoryService _organisationService;
    private readonly IPublicationService _publicationService;
    private readonly IReferenceDataCrudService<RegistrationReason> _registrationReasonService;
    private readonly IReferenceDataCrudService<ServiceOffering> _serviceOfferingService;
    private readonly AnnualStatisticsOptions _annualStatisticsConfig;
    private readonly PublicationsOptions _publicationsConfig;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogoService _logoService;

    public ProfileController(
        IReferenceDataCrudService<AnnualStatisticGroup> annualStatisticGroupService,
        IBiobankService biobankService,
        IConfigService configService,
        IReferenceDataCrudService<County> countyService,
        IReferenceDataCrudService<Country> countryService,
        IReferenceDataCrudService<Funder> funderService,
        IMapper mapper,
        INetworkService networkService,
        IOrganisationDirectoryService organisationService,
        IPublicationService publicationService,
        IReferenceDataCrudService<RegistrationReason> registrationReasonService,
        IReferenceDataCrudService<ServiceOffering> serviceOfferingService,
        IOptions<AnnualStatisticsOptions> annualStatisticsOptions,
        IOptions<PublicationsOptions> publicationsOptions,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogoService logoService)
    {
        _annualStatisticGroupService = annualStatisticGroupService;
        _biobankService = biobankService;
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
        _annualStatisticsConfig = annualStatisticsOptions.Value;
        _publicationsConfig = publicationsOptions.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _logoService = logoService;
    }
    
    #region Details
    
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> Index(int biobankId)
    {
        var model = await GetBiobankDetailsModelAsync(biobankId);

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
            await _logoService.RemoveLogo(model.BiobankId.Value);
        }
        else if (model.Logo != null)
        {
            try
            {
                var logoStream = model.Logo.ToProcessableStream();

                logoName =
                    await
                        _logoService.StoreLogo(
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
        
        // Preserve its suspended state
        biobank.IsSuspended = await _organisationService.IsSuspended(biobank.OrganisationId);

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
        await _userManager.AddClaimsAsync(await _userManager.GetUserAsync(User),new List<Claim>
        {
            new Claim(CustomClaimType.Biobank, JsonConvert.SerializeObject(new KeyValuePair<int, string>(biobank.OrganisationId, biobank.Name)))
        });
        
        // Resign in the user so their claims are repopulated.
        var user = await _userManager.GetUserAsync(User);
        await _signInManager.RefreshSignInAsync(user);

        //Logo upload (now we have the id, we can form the filename)
        if (model.Logo != null)
        {
            try
            {
                var logoStream = model.Logo.ToProcessableStream();

                //use the DTO again to update
                biobank.Logo = await
                            _logoService.StoreLogo(logoStream,
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

    /// <summary>
    /// Route to create a new biobank.
    /// </summary>
    /// <param name="biobankId">This is actually the organisation request ID,
    /// but is called this for ease of routing.</param>
    /// <returns>Edit view for Biobank.</returns>
    [Authorize(nameof(AuthPolicies.HasBiobankRequestClaim))]
    public async Task<ActionResult> Create(int biobankId)
    {
      // Check the organisation request has not been accepted
      var request = await _organisationService.GetRegistrationRequest(biobankId);
      if (request.OrganisationCreatedDate.HasValue)
      {
        // get the biobank to redirect to.
        var biobank = await _organisationService.GetByName(request.OrganisationName);
        this.SetTemporaryFeedbackMessage("This biobank has already been created.", FeedbackMessageType.Info);
        RedirectToAction("Index", "Collections", new { area = "Biobank", biobank.OrganisationId });
      }
      
      var sampleResource = await _configService.GetSiteConfigValue(ConfigKey.SampleResourceName);
      this.SetTemporaryFeedbackMessage("Please fill in the details below for your " + sampleResource + ". Once you have completed these, you'll be able to perform other administration tasks",
        FeedbackMessageType.Info);
      
      var model = await NewBiobankDetailsModelAsync(biobankId);
          
      // Reset the biobankId in the model state so the form does not populate it.
      // Ensures a new biobank is created.
      ModelState.SetModelValue("biobankId", new ValueProviderResult());
      return View("Edit", model);
    }

    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
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

        await _biobankService.AddBiobankServiceOfferings(activeServices);

        foreach (var inactiveService in model.ServiceModels.Where(x => !x.Active))
        {
            await
                _biobankService.DeleteBiobankServiceOffering(biobank.OrganisationId,
                    inactiveService.ServiceOfferingId);
        }

        //Add/Delete registration reasons
        var activeRegistrationReasons =
            model.RegistrationReasons.Where(x => x.Active).Select(x => new OrganisationRegistrationReason
            {
                RegistrationReasonId = x.RegistrationReasonId,
                OrganisationId = biobank.OrganisationId
            }).ToList();

        await _biobankService.AddBiobankRegistrationReasons(activeRegistrationReasons);

        foreach (var inactiveRegistrationReason in model.RegistrationReasons.Where(x => !x.Active))
        {
            await
                _biobankService.DeleteBiobankRegistrationReason(biobank.OrganisationId,
                    inactiveRegistrationReason.RegistrationReasonId);
        }
        var sampleResource = await _configService.GetSiteConfigValue(ConfigKey.SampleResourceName);
        this.SetTemporaryFeedbackMessage(sampleResource + " details updated!", FeedbackMessageType.Success);

        //Back to the profile to view your saved changes
        return RedirectToAction("Index", new { biobankId = biobank.OrganisationId } );
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
                await _biobankService.ListBiobankServiceOfferings(bb.OrganisationId);

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
            CountryId = bb.Country?.Id,
            CountyName = bb.County?.Value,
            CountryName = bb.Country?.Value,
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
    [Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
    public async Task<ActionResult> AddTempLogo()
    {
        if (!HttpContext.Request.Form.Files.Any())
            return BadRequest(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

        var formFile = HttpContext.Request.Form.Files["TempLogo"];

        if (formFile == null)
            return BadRequest(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

        if (formFile.Length > 1000000)
            return BadRequest(new KeyValuePair<bool, string>(false, "The file you supplied is too large. Logo image files must be 1Mb or less."));
        
        try
        {
            if (formFile.ValidateAsLogo())
            {
                var logoStream = formFile.ToProcessableStream();
                
                var resizedImage = await ImageService.ResizeImageStream(logoStream, maxX: 300, maxY: 300);
                // Set session variable so the TempLogo action can retrieve it
                HttpContext.Session.Set("TempLogo", resizedImage.ToArray());
                
                return Ok(new KeyValuePair<bool, string>(true, Url.Action("TempLogo", "Profile")));
                
            }
        }
        catch (BadImageFormatException e)
        {
            return BadRequest(new KeyValuePair<bool, string>(false, e.Message));
        }

        return BadRequest(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));
    }
    
    [HttpGet]
    [Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
    public ActionResult TempLogo()
    {
        var bytes = HttpContext.Session.Get("TempLogo");
        return File(bytes, "image/png");
    }

    [HttpPost]
    [Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
    public void RemoveTempLogo()
    {
        HttpContext.Session.Remove("TempLogo");
    }
    
    #endregion

    #region Funders

    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
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
        => (await _biobankService.ListBiobankFunders(biobankId))
            .Select(bbFunder => new FunderModel
            {
                FunderId = bbFunder.Id,
                Name = bbFunder.Value
            }).ToList();
    
    public ActionResult AddFunderSuccess(int biobankId, string name)
    {
        //This action solely exists so we can set a feedback message
    
        this.SetTemporaryFeedbackMessage($"{name} has been successfully added to your list of funders!",
            FeedbackMessageType.Success);
    
        return RedirectToAction("Funders", new { biobankId = biobankId });
    }
    
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
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
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> AddFunderAjax(int biobankId, AddFunderModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
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
    
                return BadRequest(new
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
    
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> DeleteFunder(int biobankId, int funderId, string funderName)
    {
        if (biobankId == 0)
            return RedirectToAction("Index", "Home");
    
        //remove them from the network
        await _organisationService.RemoveFunder(funderId, biobankId);
    
        this.SetTemporaryFeedbackMessage($"{funderName} has been removed from your list of funders!", FeedbackMessageType.Success);
    
        return RedirectToAction("Funders", new { biobankId = biobankId });
    }
    
    [Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
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
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> Publications(int biobankId)
    {
        //If turned off in site config
        if (await _configService.GetFlagConfigValue(ConfigKey.DisplayPublications) == false)
            return NotFound();

        return View(biobankId);
    }


    [HttpGet]
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
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

    private async Task<Publication> PublicationSearch(string publicationId, int biobankId)
    {
        // Find Given Publication
        var publications = await _publicationService.ListByOrganisation(biobankId);
        var publication = publications.FirstOrDefault(x => x.PublicationId == publicationId);

        //retrieve from EPMC if not found
        if (publication == null)
        {
            try
            {
                using var client = new HttpClient();
                var buildUrl = new UriBuilder(_publicationsConfig.EpmcApiUrl)
               
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
                
                return null;
            }

        }

        return publication;
    }

    [HttpGet]
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> RetrievePublicationsAjax(int biobankId, string publicationId)
    {
        
        if (biobankId == 0 || string.IsNullOrEmpty(publicationId))
            return Ok(new EmptyResult());
        else
        {
            // Find Publication locally
            var publications = await _publicationService.ListByOrganisation(biobankId);
            var publication = publications.FirstOrDefault(x => x.PublicationId == publicationId);

            // search online
            if (publication == null)
                publication = await PublicationSearch(publicationId, biobankId);

            return publication != null
                ? Ok(_mapper.Map<BiobankPublicationModel>(publication))
                : Ok(new EmptyResult());
        }
    }

    [HttpPost]
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> AddPublicationAjax(int biobankId, string publicationId)
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
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> ClaimPublicationAjax(int biobankId, string publicationId, bool accept)
    {
        if (biobankId == 0 || string.IsNullOrEmpty(publicationId))
            return Ok(new EmptyResult());

        // Update Publication
        var publication = await _publicationService.Claim(publicationId, biobankId, accept);

        return Ok(_mapper.Map<BiobankPublicationModel>(publication));
    }

    public ActionResult AddPublicationSuccessFeedback(int biobankId, string publicationId)
    {
        this.SetTemporaryFeedbackMessage($"The publication with PubMed ID \"{publicationId}\" has been added successfully.", FeedbackMessageType.Success);
        return RedirectToAction("Publications", new { biobankId = biobankId });
    }
    
    #endregion
    
    #region Annual Stats

    [HttpGet]
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<ActionResult> AnnualStats(int biobankId)
        => View(new BiobankAnnualStatsModel
        {
            AnnualStatisticGroups = await _annualStatisticGroupService.List(),
            BiobankAnnualStatistics = (await _organisationService.Get(biobankId)).OrganisationAnnualStatistics,
            BiobankId = biobankId
        });

    [HttpPost]
    [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
    public async Task<JsonResult> UpdateAnnualStatAjax(int biobankId, AnnualStatModel model)
    {
        if (model.Year > DateTime.Now.Year)
            ModelState.AddModelError("", $"Year value for annual stat cannot be in the future.");

        var annualStatsStartYear = int.Parse(_annualStatisticsConfig.AnnualStatsStartYear);
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
        await _biobankService.UpdateOrganisationAnnualStatistic(
            biobankId,
            model.AnnualStatisticId,
            model.Value,
            model.Year);

        return Json(new
        {
            success = true
        });
    }

    #endregion
    
}
