using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Biobanks.Directory.Data;
using Biobanks.Entities.Data;

namespace Biobanks.Services
{
    /// <summary>
    /// This Service makes HTTP calls to an API to generate Analytics Reports for the Directory
    /// </summary>
    public class AnalyticsService : IDisposable, IAnalyticsService
    {
        // TODO: genericise this for Directory calls to the Directory API
        private readonly string _apiUrl = ConfigurationManager.AppSettings["DirectoryApiUrl"] ?? "";
        private readonly string _apiClientId = ConfigurationManager.AppSettings["DirectoryApiClientId"] ?? "";
        private readonly string _apiClientSecret = ConfigurationManager.AppSettings["DirectoryApiClientSecret"] ?? "";

        private readonly HttpClient _client;
        private readonly BiobanksDbContext _db;

        public AnalyticsService(BiobanksDbContext db)
        {
            _db = db;
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

        private ProfileStatusDTO GetProfileStatus(Organisation biobank)
        {
            //can split into two functions that returns status code and status message
            int collectionCount = biobank.Collections.Count;
            int capabilitiesCount = biobank.DiagnosisCapabilities.Count;

            bool missingSampleSet = false;
            ProfileStatusDTO profileStatus = new ProfileStatusDTO();

            foreach (var col in biobank.Collections)
            {
                var collection = _db.Collections.Include("SampleSets").SingleOrDefault(x => x.CollectionId == col.CollectionId);
                if (collection.SampleSets.Count == 0)
                { // Check if any collection exists without a sample set
                    missingSampleSet = true;
                    break;
                }
            }

            //Check Collection Status
            if (collectionCount == 0)
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

        public async Task<BiobankAnalyticReportDTO> GetBiobankReport(int Id, int year, int quarter, int period)
        {
            var biobank = _db.Organisations.SingleOrDefault(x => x.OrganisationId == Id);
            var biobankId = biobank.OrganisationExternalId;

            if (biobank is null) throw new KeyNotFoundException();

            var endpoint = $"analytics/{year}/{quarter}/{period}/{biobankId}";
            var response = await _client.GetAsync(endpoint);
            var contents = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var report = JsonConvert.DeserializeObject<BiobankAnalyticReportDTO>(contents);

            var profileStatus = GetProfileStatus(biobank);
            report.Name = biobank.Name;
            report.Logo = biobank.Logo;
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
