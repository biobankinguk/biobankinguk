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
        private readonly ILogger<FetchPublicationsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBiobankService _biobank;

        public BatchFunction(ILogger<FetchPublicationsService> logger, IServiceScopeFactory scopeFactory, IBiobankService biobank)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _biobank = biobank;
        }

        

        [FunctionName("BatchFunction")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            FetchPublicationsService fetchPublicationsService = new FetchPublicationsService(_logger, _scopeFactory);
            CancellationToken cancellationToken;

            fetchPublicationsService.StartAsync(cancellationToken);


            fetchPublicationsService.StopAsync(cancellationToken);

            


        }
    }
}
