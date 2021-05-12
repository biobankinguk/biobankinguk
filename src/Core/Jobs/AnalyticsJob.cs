using Biobanks.Analytics.Services.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AnalyticsJob
    {

        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsJob(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        public async Task Run()
            => await _googleAnalyticsReadService.UpdateAnalyticsData();

    }
}
