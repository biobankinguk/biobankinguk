using Biobanks.Analytics;
using Biobanks.Analytics.Services;
using Biobanks.Analytics.Services.Contracts;

using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AnalyticsJob
    {
        private readonly IGoogleAnalyticsReportingService _ga;
        private readonly IAnalyticsService _analytics;
        private readonly AnalyticsOptions _config;

        public AnalyticsJob(
            IGoogleAnalyticsReportingService ga,
            IAnalyticsService analytics,
            IOptions<AnalyticsOptions> options)
        {
            _ga = ga;
            _analytics = analytics;
            _config = options.Value;
        }

        public async Task Run()
        {
            // Get Latest Analytics Data
            var lastBiobankEntry = await _analytics.GetLatestOrganisationAnalyticsTimestamp();
            var lastEventEntry = await _analytics.GetLatestAnalyticsEventTimestamp();
            var lastMetricEntry = await _analytics.GetLatestAnalyticsMetricTimestamp();

            // Find Lastest Entry Date
            var lastentry = new[] { lastBiobankEntry, lastEventEntry, lastMetricEntry }.Max();

            // Set Date Range based on lastentry
            var startDate = lastentry == DateTimeOffset.MinValue
                ? DateTimeOffset.Parse(_config.StartDate)
                : lastentry;

            var endDate = DateTimeOffset.Now;

            // As long as lastentry is within an acceptable range, fetch the data
            if(lastentry >= DateTimeOffset.MinValue && lastentry < DateTimeOffset.Now)
            {
                await _ga.DownloadAllBiobankData(startDate, endDate);
                await _ga.DownloadDirectoryData(startDate, endDate);
            }
        }
    }
}
