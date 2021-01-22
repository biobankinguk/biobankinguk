using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Directory.Services.Dto;
using Newtonsoft.Json;

namespace Biobanks.Directory.Services
{
    public class AnalyticsReportGenerator : IDisposable, IAnalyticsReportGenerator
    {
        private readonly string _analyticsUrl = ConfigurationManager.AppSettings["AnalyticsServiceUrl"] ?? "";
        private readonly string _analyticsKey = ConfigurationManager.AppSettings["AnalyticsFunctionKey"] ?? "";

        // Service for reading from the seperate analytics service - ie. The Azure Function App
        private readonly HttpClient _client;
        private readonly IBiobankReadService _biobankReadService;

        public AnalyticsReportGenerator(IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
            _client = new HttpClient();

            if (!string.IsNullOrEmpty(_analyticsUrl))
            {
                _client.BaseAddress = new Uri(_analyticsUrl);
            }
            if (!string.IsNullOrEmpty(_analyticsKey))
            {
                _client.DefaultRequestHeaders.Add("x-functions-key", _analyticsKey);
            }
        }

        public async Task<ProfileStatusDTO> GetProfileStatus(string biobankId)
        {
            //can split into two functions that returns status code and status message
            var bb = await _biobankReadService.GetBiobankByExternalIdAsync(biobankId);
            int collectionCount = bb.Collections.Count;
            int capabilitiesCount = bb.DiagnosisCapabilities.Count;

            bool missingSampleSet = false;
            ProfileStatusDTO profileStatus = new ProfileStatusDTO();

            foreach (var col in bb.Collections)
            {
                var ss = await _biobankReadService.GetCollectionWithSampleSetsByIdAsync(col.CollectionId);
                if (ss.SampleSets.Count == 0)
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

            //Check HRA/HTA Status
            if (bb.EthicsRegistration == null || bb.HtaLicence == null)
            {
                profileStatus.HRA_HTAStatus = 0;
                profileStatus.HRA_HTAStatusMessage = "HRA/HTA number missing";
            }
            else
            {
                profileStatus.HRA_HTAStatus = 1;
                profileStatus.HRA_HTAStatusMessage = "HRA/HTA number has been recorded";
            }

            return profileStatus;
        }

        public async Task<BiobankAnalyticReportDTO> GetBiobankReport(int Id, int year, int quarter, int period)
        {
            var bb = await _biobankReadService.GetBiobankByIdAsync(Id);
            var biobankId = bb.OrganisationExternalId;

            if (bb == null)
                return new BiobankAnalyticReportDTO
                {
                    Error = new ErrorStatusModelDTO { ErrorCode = -1, ErrorMessage = "Biobank Not Found" }
                };

            // Build endpoint
            var apikey = ConfigurationManager.AppSettings["AnalyticsFunctionKey"]; 
            var endpoint = $"api/GetAnalyticsReport/{biobankId}/{year}/{quarter}/{period}";

            try
            {
                // Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                //check response is Successful
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // Fail safely - will render a warning message in the view?
                    return new BiobankAnalyticReportDTO
                    {
                        Error = new ErrorStatusModelDTO { ErrorCode = -2, ErrorMessage = "API request failed!" }
                    };  //TODO:change to use HttpStatusCode codes?
                }

                // Deserialize and Map array
                var report = JsonConvert.DeserializeObject<BiobankAnalyticReportDTO>(contents);

                // Add extra report info
                var profileStatus = await GetProfileStatus(biobankId);
                report.Name = bb.Name;
                report.Logo = bb.Logo;
                report.BiobankStatus = profileStatus;

                return report;
            }
            catch (Exception)
            {
                return new BiobankAnalyticReportDTO
                {
                    Error = new ErrorStatusModelDTO { ErrorCode = -2, ErrorMessage = "API request failed!" }
                }; //TODO:change to use HttpStatusCode codes?
            }
        }

        public async Task<DirectoryAnalyticReportDTO> GetDirectoryReport(int year, int quarter, int period)
        {
            // Build endpoint
            var apikey = ConfigurationManager.AppSettings["AnalyticsFunctionKey"];
            var endpoint = $"api/GetDirectoryAnalyticsReport/{year}/{quarter}/{period}";

            try
            {
                // Make request
                var response = await _client.GetAsync(endpoint);
                var contents = await response.Content.ReadAsStringAsync();

                //check response is Successful
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // Fail safely - will render a warning message in the view?
                    return new DirectoryAnalyticReportDTO
                    {
                        Error = new ErrorStatusModelDTO { ErrorCode = -2, ErrorMessage = "API request failed!" }
                    };  //TODO:change to use HttpStatusCode codes?
                }
                    
                // Deserialize and Map array
                var report = JsonConvert.DeserializeObject<DirectoryAnalyticReportDTO>(contents);
                return report;
            }
            catch (Exception)
            {
                // Fail safely - will render a warning message in the view?
                return new DirectoryAnalyticReportDTO
                {
                    Error = new ErrorStatusModelDTO { ErrorCode = -2, ErrorMessage = "API request failed!" }
                }; //TODO:change to use HttpStatusCode codes?
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
