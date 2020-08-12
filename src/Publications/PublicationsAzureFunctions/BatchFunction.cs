using System;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Publications.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Publications.Services.Contracts;

namespace PublicationsAzureFunctions
{

    public class BatchFunction
    {
        private FetchPublicationsService _fetchPublicationsService;

        public BatchFunction(FetchPublicationsService fetchPublicationsService)
        {
            _fetchPublicationsService = fetchPublicationsService;
        }

        

        [FunctionName("BatchFunction")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            CancellationToken cancellationToken;

            _fetchPublicationsService.StartAsync(cancellationToken);


            _fetchPublicationsService.StopAsync(cancellationToken);

            
        }
    }
}
