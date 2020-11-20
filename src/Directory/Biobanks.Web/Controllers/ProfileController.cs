using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Directory.Entity.Data;
using Directory.Identity.Constants;
using Directory.Services.Contracts;
using Biobanks.Web.Models.Profile;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Directory.Data.Constants;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class ProfileController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;

        public ProfileController(
            IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
        }

        public ActionResult Biobanks()
        {
            var model = _biobankReadService.GetOrganisations();
            return View(model);
        }

        public async Task<ActionResult> Biobank(string id)
        {
            //get the biobank
            var bb = await _biobankReadService.GetBiobankByExternalIdAsync(id);

            if(bb == null) return new HttpNotFoundResult();

            if (bb.IsSuspended)
            {
                //Allow ADAC or this Biobank's admins to view the profile
                if (CurrentUser.BiobankIds.Contains(bb.OrganisationId.ToString()) && User.IsInRole(Role.BiobankAdmin.ToString()) ||
                    User.IsInRole(Role.ADAC.ToString()))
                {
                    //But alert them that the bb is suspended
                    SetTemporaryFeedbackMessage(
                        "This biobank is currently suspended, so this public profile will not be accessible to non-admins.",
                        FeedbackMessageType.Warning);
                }
                else
                {
                    //Anyone else gets a 404
                    return new HttpNotFoundResult();
                }
            }

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
                CountyName = bb.County?.Name,
                CountryName = bb.Country.Name,
                ContactNumber = bb.ContactNumber,
                LastUpdated = bb.LastUpdated,
                NetworkMembers = (await _biobankReadService.GetNetworksByBiobankIdAsync(bb.OrganisationId)).Select(
                    x => new NetworkMemberModel
                    {
                        Id = x.NetworkId,
                        Name = x.Name,
                        Logo = x.Logo,
                        Description = x.Description
                    }).ToList(),
                CapabilityDiagnoses = (await _biobankReadService.ListCapabilitiesAsync(bb.OrganisationId)).Select(
                    x => new CapabilityModel
                    {
                        Id = x.DiagnosisCapabilityId,
                        Diagnosis = x.Diagnosis.Description,
                        Protocols = x.SampleCollectionMode.Description
                    })
                    .Select(x => x.Diagnosis)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                CollectionDiagnoses = (await _biobankReadService.ListCollectionsAsync(bb.OrganisationId)).Select(
                    x => new CollectionModel
                    {
                        Id = x.CollectionId,
                        Diagnosis = x.Diagnosis.Description,
                        SampleSetsCount = x.SampleSets.Count,
                        StartYear = x.StartDate.Year,
                        MaterialTypes = GetMaterialTypeSummary(x.SampleSets)
                    })
                    .Select(x => x.Diagnosis)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),
                Services = (await _biobankReadService.ListBiobankServiceOfferingsAsync(bb.OrganisationId))
                    .OrderBy(x => x.ServiceOffering.SortOrder)
                    .Select(x => x.ServiceOffering.Name)
                    .ToList(),
               
                BiobankAnnualStatistics = bb.OrganisationAnnualStatistics,
                AnnualStatisticGroups = await _biobankReadService.GetAnnualStatisticGroupsAsync()
            };

            return View(model);
        }

        public async Task<ActionResult> Network(int id)
        {
            var nw = await _biobankReadService.GetNetworkByIdAsync(id);

            var model = new NetworkModel
            {
                Id = nw.NetworkId,
                Name = nw.Name,
                Description = nw.Description,
                Url = nw.Url,
                Logo = nw.Logo,
                ContactEmail = nw.Email,
                SopStatus = nw.SopStatus.Description,
                BiobankMembers = (await _biobankReadService.GetBiobanksByNetworkIdAsync(id)).Select(
                    x => new BiobankMemberModel
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
            if (!(await _biobankReadService.GetSiteConfigStatus(ConfigKey.DisplayPublications)))
                return HttpNotFound();

            // Get the Organisation
            var bb = await _biobankReadService.GetBiobankByExternalIdAsync(id);

            if (bb == null)
            {
                return new HttpNotFoundResult();
            }
            else
            {
                // Get accepted publications
                var publications = await _biobankReadService.GetAcceptedOrganisationPublicationsAsync(bb);

                // TODO: Migrate/Recreate Model?
                var model = new BiobankPublicationsModel
                {
                    ExternalId = bb.OrganisationExternalId,
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

        private static string GetMaterialTypeSummary(IEnumerable<CollectionSampleSet> sampleSets)
        {
            var materialTypes = new List<string>();

            sampleSets.ToList().ForEach(
                x => x.MaterialDetails.ToList().ForEach(
                    y => materialTypes.Add(y.MaterialType.Description)));

            //return first 3, slash separated
            var result = string.Join(" / ", materialTypes.Distinct().OrderBy(x => x).Take(3));

            return string.IsNullOrEmpty(result)
                ? "N/A"
                : result;
        }
 
    }
}
