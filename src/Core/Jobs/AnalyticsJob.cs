using Biobanks.Analytics.Services.Contracts;
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AnalyticsJob
    {

        private readonly string _startDate = "2016-01-01"; //TODO: Get via configuration
        private readonly string _dateFormat = "yyyy-MM-dd";

        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsJob(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        public async Task Run()
        {
            // Get Latest Analytics Data
            var lastBiobankEntry = await _googleAnalyticsReadService.GetLatestBiobankEntry();
            var lastEventEntry = await _googleAnalyticsReadService.GetLatestMetricEntry();
            var lastMetricEntry = await _googleAnalyticsReadService.GetLatestEventEntry();

            // Find Lastest Entry Date
            var lastentry = new[] { lastBiobankEntry, lastEventEntry, lastMetricEntry }.Max();

            // No Previous Analytics
            if (lastentry == DateTimeOffset.MinValue)
            {
                var dateRange = new[] { new DateRange { StartDate = _startDate, EndDate = DateTimeOffset.Now.ToString(_dateFormat) } };

                //TODO: Refactor this service method to have parameters (DateTimeOffset start, DateTimeOffset end)
                await _googleAnalyticsReadService.DownloadDirectoryData(dateRange);
            }
            // If last entry is in the past
            else if (lastentry > DateTimeOffset.MinValue && lastentry < DateTimeOffset.Now)
            {
                var dateRange = new[] { new DateRange { StartDate = lastentry.ToString(_dateFormat), EndDate = DateTimeOffset.Now.ToString(_dateFormat) } };

                await _googleAnalyticsReadService.DownloadAllBiobankData(dateRange);
                await _googleAnalyticsReadService.DownloadDirectoryData(dateRange);
            }
        }
    }
}
