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

            // No Previous Analytics
            if (lastentry == DateTimeOffset.MinValue)
            {
                var dateRanges = new[] { _ga.DateRangeFromBounds(_config.StartDate, DateTimeOffset.Now) };

                await _ga.DownloadDirectoryData(dateRanges);
            }
            // If last entry is in the past
            else if (lastentry > DateTimeOffset.MinValue && lastentry < DateTimeOffset.Now)
            {
                var dateRanges = new[] { _ga.DateRangeFromBounds(lastentry, DateTimeOffset.Now) };

                await _ga.DownloadAllBiobankData(dateRanges);
                await _ga.DownloadDirectoryData(dateRanges);
            }
        }
    }
}
