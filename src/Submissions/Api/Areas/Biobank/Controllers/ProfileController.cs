using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Extensions;
using Biobanks.Submissions.Api.Models.Directory;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    private readonly Mapper _mapper;
    private readonly NetworkService _networkService;
    private readonly OrganisationDirectoryService _organisationService;
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
        Mapper mapper,
        NetworkService networkService,
        OrganisationDirectoryService organisationService,
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
        _mapper = mapper;
        _networkService = networkService;
        _organisationService = organisationService;
        _registrationReasonService = registrationReasonService;
        _serviceOfferingService = serviceOfferingService;
        _userManager = userManager;
    }

    [Authorize(CustomClaimType.Biobank)]
    public async Task<ActionResult> Index()
    {
        var model = await GetBiobankDetailsModelAsync();

        var biobankId = SessionHelper.GetBiobankId(MetricDimensionNames.TelemetryContext.Session);

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
        _userManager.AddClaimsAsync(User,new List<Claim>
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
            this.SetTemporaryFeedbackMessage("Please fill in the details below for your " + sampleResource + ". Once you have completed these, you'll be able to perform other administration tasks",
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

    private async Task<BiobankDetailsModel> NewBiobankDetailsModelAsync()
    {
        //the biobank doesn't exist yet, but a request should, so we can get the name
        var request = await _organisationService.GetRegistrationRequest(SessionHelper.GetBiobankId(MetricDimensionNames.TelemetryContext.Session));

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
        var bb = await _organisationService.Get(SessionHelper.GetBiobankId(MetricDimensionNames.TelemetryContext.Session));

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
    
}
