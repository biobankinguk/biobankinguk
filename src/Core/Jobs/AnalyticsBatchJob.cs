using Biobanks.Analytics.Services.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AnalyticsBatchJob
    {

        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsBatchJob(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        public async Task Run()
            => await _googleAnalyticsReadService.UpdateAnalyticsData();

    }
}
