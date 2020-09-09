using System;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Publications.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Publications.Services.Contracts;
using System.Threading.Tasks;

namespace PublicationsAzureFunctions
{
    public class BatchFunction
    {
        private FetchPublicationsService _fetchPublicationsService;
        private readonly CancellationToken cancellationToken;

        public BatchFunction(FetchPublicationsService fetchPublicationsService)
        {
            _fetchPublicationsService = fetchPublicationsService;
        }

        //Configured to run every 24 hours
        [FunctionName("BatchFunction")]
        public async Task Run([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //Pulls Biobanks from directory (test), gets publications from API and pushes to Azure DB
            await _fetchPublicationsService.StartAsync(cancellationToken);
            await _fetchPublicationsService.StopAsync(cancellationToken);

            log.LogInformation($"C# Timer trigger function executed successfully");
        }
    }
}
