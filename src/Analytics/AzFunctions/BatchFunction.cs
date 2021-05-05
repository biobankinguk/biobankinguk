using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Biobanks.Analytics.Core.Contracts;

namespace Biobanks.Analytics.AzFunctions
{
    public class BatchFunction
    {
        private IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public BatchFunction(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        [FunctionName("BatchFunction")]
        //Configured to run every quarter (At 00:00 on the 1st day in every 3rd month)
        public async Task Run([TimerTrigger("0 0 0 1 */3 *", RunOnStartup = false)] ILogger log) //remove runonstartup before deploying
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTimeOffset.Now}");

            //Pulls Biobanks from directory (test), gets Analytics from API and pushes to Azure DB
            await _googleAnalyticsReadService.UpdateAnalyticsData();

            log.LogInformation($"C# Timer trigger function executed successfully");
        }
    }
}
