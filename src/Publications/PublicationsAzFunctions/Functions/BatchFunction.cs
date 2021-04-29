using System;
using System.Threading;
using System.Threading.Tasks;
using Biobanks.Publications.Services.Hosted;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PublicationsAzFunctions.Types;

namespace PublicationsAzFunctions.Functions
{
    public class BatchFunction
    {
        private FetchPublicationsService _fetchPublicationsService;
        private FetchAnnotationsService _fetchAnnotationsService;
        private readonly CancellationToken cancellationToken;

        public BatchFunction(FetchPublicationsService fetchPublicationsService, FetchAnnotationsService fetchAnnotationsService)
        {
            _fetchPublicationsService = fetchPublicationsService;
            _fetchAnnotationsService = fetchAnnotationsService;
        }


        [Function("BatchFunction")]
        public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("BatchFunction");
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //Pulls Biobanks from directory, gets publications from API and pushes to Azure DB
            await _fetchPublicationsService.StartAsync(cancellationToken);
            await _fetchPublicationsService.StopAsync(cancellationToken);

            //Pulls Publications from directory, gets annotations from API and pushes to Azure DB
            await _fetchAnnotationsService.StartAsync(cancellationToken);
            await _fetchAnnotationsService.StopAsync(cancellationToken);

            logger.LogInformation($"C# Timer trigger function executed successfully");
        }
    }

    
}
