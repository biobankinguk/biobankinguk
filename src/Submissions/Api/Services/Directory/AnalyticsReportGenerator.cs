using System.Threading.Tasks;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Collections.Generic;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Analytics.Dto;

namespace Biobanks.Submissions.Api.Services.Directory
{
    /// <summary>
    /// This Service generates Analytics Reports for a Biobank
    /// </summary>
    public class AnalyticsReportGenerator : IAnalyticsReportGenerator
    {
        private readonly IOrganisationDirectoryService _organisationService;
        private readonly IOrganisationReportGenerator _organisationReports;
        private readonly ICollectionService _collectionService;

        public AnalyticsReportGenerator(
            ICollectionService collectionService,
            IOrganisationDirectoryService organisationService,
            IOrganisationReportGenerator organisationReports)
        {
            _collectionService = collectionService;
            _organisationService = organisationService;
            _organisationReports = organisationReports;
        }

        public async Task<ProfileStatusDTO> GetProfileStatus(string biobankId)
        {
            //can split into two functions that returns status code and status message
            var bb = await _organisationService.GetByExternalId(biobankId);
            int collectionCount = bb.Collections.Count;
            
            int capabilitiesCount = bb.DiagnosisCapabilities.Count;

            bool missingSampleSet = false;
            ProfileStatusDTO profileStatus = new ProfileStatusDTO();

            foreach (var col in bb.Collections)
            {
                // Check if any collection exists without a sample set
                if (!await _collectionService.HasSampleSets(col.CollectionId))
                {
                    missingSampleSet = true;
                    break;
                }
            }

            //Check Collection Status
            if (collectionCount == 0 )
            {
                profileStatus.CollectionStatus = 0;
                profileStatus.CollectionStatusMessage = "No collections registered";
            }
            else if (collectionCount > 0 && missingSampleSet == false)
            {
                profileStatus.CollectionStatus = 1;
                profileStatus.CollectionStatusMessage = "Collections registered and no missing samples";
            }
            else
            {
                profileStatus.CollectionStatus = 0;
                profileStatus.CollectionStatusMessage = "Collections registered but missing samples";
            }

            //Check Capability Status
            if (capabilitiesCount == 0)
            {
                profileStatus.CapabilityStatus = 0;
                profileStatus.CapabilityStatusMessage = "No Capabilities registered";
            }
            else
            {
                profileStatus.CapabilityStatus = 1;
                profileStatus.CapabilityStatusMessage = "Capabilities registered";
            }

            return profileStatus;
        }

        public async Task<OrganisationReportDto> GetBiobankReport(int Id, int year, int quarter, int period)
        {
            var bb = await _organisationService.Get(Id);
            var biobankId = bb.OrganisationExternalId;

            if (bb is null) throw new KeyNotFoundException();

            var report = await _organisationReports.GetReport(biobankId, year, quarter, period);

            var profileStatus = await GetProfileStatus(biobankId);
            report.Name = bb.Name;
            report.Logo = bb.Logo;
            report.BiobankStatus = profileStatus;

            return report;
        }
    }
}
