using System;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Analytics.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Services.Contracts;
using System.Threading.Tasks;

namespace AnalyticsAzureFunctions
{

    public class BatchFunction
    {
        private GoogleAnalyticsReadService _googleAnalyticsReadService;
        private readonly CancellationToken cancellationToken;

        public BatchFunction(GoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

       
        [FunctionName("BatchFunction")]
        //Configured to run every quarter (At 00:00 on the 1st day in every 3rd month)
        public async Task Run([TimerTrigger("0 0 0 1 */3 *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //Pulls Biobanks from directory (test), gets Analytics from API and pushes to Azure DB
            await _googleAnalyticsReadService.StartAsync(cancellationToken);

            await _googleAnalyticsReadService.StopAsync(cancellationToken);

            log.LogInformation($"C# Timer trigger function executed successfully");
            



        }
    }
}
