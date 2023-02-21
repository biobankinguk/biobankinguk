using Biobanks.Entities.Data.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory;
using System.Threading.Tasks;
using System.Collections.Generic;
using Biobanks.Entities.Data;
using System.Linq;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Models.Profile;
using Biobanks.Submissions.Api.Config;

namespace Biobanks.Submissions.Api.Controllers.Directory
{
    [AllowAnonymous]
    public class ProfileController : Controller
    {
        private readonly IReferenceDataCrudService<AnnualStatisticGroup> _annualStatisticGroupService;
        private readonly ICollectionService _collectionService;
        private readonly IPublicationService _publicationService;
        private readonly INetworkService _networkService;
        private readonly IOrganisationDirectoryService _organisationService;
        private readonly IBiobankService _biobankService;
        private readonly IConfigService _configService;
        private readonly ICapabilityService _capabilityService;

        public ProfileController(
            IReferenceDataCrudService<AnnualStatisticGroup> annualStatisticGroupService,
            ICollectionService collectionService,
            IPublicationService publicationService,
            INetworkService networkService,
            IOrganisationDirectoryService organisationService,
            IBiobankService biobankService,
            IConfigService configService,
            ICapabilityService capabilityService)
        {
            _annualStatisticGroupService = annualStatisticGroupService;
            _networkService = networkService;
            _organisationService = organisationService;
            _publicationService = publicationService;
            _biobankService = biobankService;
            _collectionService = collectionService;
            _configService = configService;
            _capabilityService = capabilityService;
        }

        public async Task<ActionResult> Biobanks()
            => View(await _organisationService.List());

        public async Task<ActionResult> Biobank(string id)
        {
            //get the biobank
            var bb = await _organisationService.GetByExternalId(id);
            if (bb is null) return NotFound();

            // TODO: Update when the user model is added.
            // if (bb.IsSuspended)
            // {
            //     //Allow ADAC or this Biobank's admins to view the profile
            //     if (CurrentUser.Biobanks.ContainsKey(bb.OrganisationId) && User.IsInRole(Role.BiobankAdmin.ToString()) ||
            //         User.IsInRole(Role.ADAC.ToString()))
            //     {
            //         //But alert them that the bb is suspended
            //         this.SetTemporaryFeedbackMessage(
            //             "This biobank is currently suspended, so this public profile will not be accessible to non-admins.",
            //             FeedbackMessageType.Warning);
            //     }
            //     else
            //     {
            //         //Anyone else gets a 404
            //         return new HttpNotFoundResult();
            //     }
            // }
            
            var model = new BiobankModel
            {
                Description = bb.Description,
                ExternalId = id,
                Url = bb.Url,
                Logo = bb.Logo,
                Name = bb.Name,
                ContactEmail = bb.ContactEmail,
                AddressLine1 = bb.AddressLine1,
                AddressLine2 = bb.AddressLine2,
                AddressLine3 = bb.AddressLine3,
                AddressLine4 = bb.AddressLine4,
                City = bb.City,
                PostCode = bb.PostCode,
                CountyName = bb.County?.Value,
                CountryName = bb.Country.Value,
                ContactNumber = bb.ContactNumber,
                LastUpdated = bb.LastUpdated,
                NetworkMembers = (await _networkService.ListByOrganisationId(bb.OrganisationId)).Select(
                    x => new NetworkMemberModel
                    {
                        Id = x.NetworkId,
                        Name = x.Name,
                        Logo = x.Logo,
                        Description = x.Description
                    }).ToList(),
                CapabilityOntologyTerms = (await _capabilityService.ListCapabilitiesAsync(bb.OrganisationId)).Select(
                    x => new CapabilityModel
                    {
                        Id = x.DiagnosisCapabilityId,
                        OntologyTerm = x.OntologyTerm.Value,
                        Protocols = x.SampleCollectionMode.Value
                    })
                    .Select(x => x.OntologyTerm)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                CollectionOntologyTerms = (await _collectionService.List(bb.OrganisationId)).Select(
                    x => new CollectionModel
                    {
                        Id = x.CollectionId,
                        OntologyTerm = x.OntologyTerm.Value,
                        SampleSetsCount = x.SampleSets.Count,
                        StartYear = x.StartDate.Year,
                        MaterialTypes = GetMaterialTypeSummary(x.SampleSets)
                    })
                    .Select(x => x.OntologyTerm)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                Services = (await _biobankService.ListBiobankServiceOfferingsAsync(bb.OrganisationId))
                    .OrderBy(x => x.ServiceOffering.SortOrder)
                    .Select(x => x.ServiceOffering.Value)
                    .ToList(),
                
                BiobankAnnualStatistics = bb.OrganisationAnnualStatistics,
                AnnualStatisticGroups = await _annualStatisticGroupService.List()
            };

            return View(model);
        }

        public async Task<ActionResult> Network(int id)
        {
            var network = await _networkService.Get(id);
            var organisations = await _organisationService.ListByNetworkId(id);

            var model = new NetworkModel
            {
                Id = network.NetworkId,
                Name = network.Name,
                Description = network.Description,
                Url = network.Url,
                Logo = network.Logo,
                ContactEmail = network.Email,
                SopStatus = network.SopStatus.Value,
                BiobankMembers = organisations.Select(x => new BiobankMemberModel
                {
                    Id = x.OrganisationId,
                    ExternalId = x.OrganisationExternalId,
                    Name = x.Name,
                    Logo = x.Logo
                }).
                    ToList()
            };


            return View(model);
        }

        public async Task<ActionResult> Publications(string id)
        {
            //If turned off in site config
            if (await _configService.GetFlagConfigValue(ConfigKey.DisplayPublications) == false)
                return NotFound();

            // Get the Organisation
            var bb = await _organisationService.GetByExternalId(id);

            if (bb is null) return NotFound();

            else
            {
                // Get accepted publications
                var publications = await _publicationService.ListByOrganisation(bb.OrganisationId, acceptedOnly: true);

                var model = new BiobankPublicationsModel
                {
                    ExternalId = bb.OrganisationExternalId,
                    ExcludePublications = bb.ExcludePublications,
                    Publications = publications.Select(x => new BiobankPublicationModel
                    {
                        Title = x.Title,
                        Authors = x.Authors,
                        Year = x.Year,
                        Journal = x.Journal,
                        DOI = x.DOI,
                    })
                .ToList()
                };

                return View(model);
            }
        }

        private static string GetMaterialTypeSummary(IEnumerable<SampleSet> sampleSets)
        {
            var materialTypes = new List<string>();

            sampleSets.ToList().ForEach(
                x => x.MaterialDetails.ToList().ForEach(
                    y => materialTypes.Add(y.MaterialType.Value)));

            //return first 3, slash separated
            var result = string.Join(" / ", materialTypes.Distinct().OrderBy(x => x).Take(3));

            return string.IsNullOrEmpty(result)
                ? "N/A"
                : result;
        }

    }
}
