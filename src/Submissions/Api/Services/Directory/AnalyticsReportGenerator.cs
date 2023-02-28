using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Configuration;
using Biobanks.Shared.Services.Contracts;
using System.Net.Http;
using System.Text;
using Biobanks.Submissions.Api.Services.Directory.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;
using Biobanks.Analytics.Services.Contracts;

namespace Biobanks.Submissions.Api.Services.Directory
{
    /// <summary>
    /// This Service makes HTTP calls to an API to generate Analytics Reports for the Directory
    /// </summary>
    public class AnalyticsReportGenerator : IDisposable, IAnalyticsReportGenerator
    {
        // TODO: genericise this for Directory calls to the Directory API
        private readonly string _apiUrl = ConfigurationManager.AppSettings["DirectoryApiUrl"] ?? "";
        private readonly string _apiClientId = ConfigurationManager.AppSettings["DirectoryApiClientId"] ?? "";
        private readonly string _apiClientSecret = ConfigurationManager.AppSettings["DirectoryApiClientSecret"] ?? "";

        private readonly IOrganisationDirectoryService _organisationService;
    private readonly IOrganisationReportGenerator _organisationReports;
    private readonly ICollectionService _collectionService;

        private readonly HttpClient _client;


        public AnalyticsReportGenerator(
            ICollectionService collectionService,
            IOrganisationDirectoryService organisationService,
            IOrganisationReportGenerator organisationReports)
        {
            _collectionService = collectionService;
            _organisationService = organisationService;
      _organisationReports = organisationReports;
      _client = new HttpClient();

            if (!string.IsNullOrEmpty(_apiUrl))
            {
                _client.BaseAddress = new Uri(_apiUrl);

                if (new[] { _apiClientId, _apiClientSecret }.All(x => !string.IsNullOrWhiteSpace(x)))
                {
                    _client.DefaultRequestHeaders.Add("Authorization",
                        $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_apiClientId}:{_apiClientSecret}"))}");
                }
            }
        }

        public async Task<Analytics.Dto.ProfileStatusDTO> GetProfileStatus(string biobankId)
        {
            //can split into two functions that returns status code and status message
            var bb = await _organisationService.GetByExternalId(biobankId);
            int collectionCount = bb.Collections.Count;
            
            int capabilitiesCount = bb.DiagnosisCapabilities.Count;

            bool missingSampleSet = false;
            Analytics.Dto.ProfileStatusDTO profileStatus = new Analytics.Dto.ProfileStatusDTO();

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

        public async Task<Analytics.Dto.OrganisationReportDto> GetBiobankReport(int Id, int year, int quarter, int period)
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

        public async Task<DirectoryAnalyticReportDTO> GetDirectoryReport(int year, int quarter, int period)
        {
            var endpoint = $"analytics/{year}/{quarter}/{period}";
            var response = await _client.GetAsync(endpoint);
            var contents = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<DirectoryAnalyticReportDTO>(contents);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
