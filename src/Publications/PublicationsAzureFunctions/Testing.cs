using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Publications.Services.Contracts;
using Publications.Services.Hosted;
using Publications.Services;
using System.Threading;

namespace PublicationsAzureFunctions
{
    public class Testing
    {

        private IRecommendationsService _recommendationsService;
        private FetchAnnotationsService _fetchAnnotationsService;
        private FetchPublicationsService _fetchPublicationsService;
        private readonly CancellationToken cancellationToken;
        private IBiobankReadService _biobankReadService;
        public Testing(IRecommendationsService recommendationsService, FetchPublicationsService fetchPublicationsService, FetchAnnotationsService fetchAnnotationsService, IBiobankReadService biobankReadService)
        {
            _recommendationsService = recommendationsService;
            _fetchPublicationsService = fetchPublicationsService;
            _fetchAnnotationsService = fetchAnnotationsService;
            _biobankReadService = biobankReadService;
        }

        [FunctionName("Testing")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //Pulls Publications from directory (test), gets annotations from API and pushes to Azure DB
            //await _fetchAnnotationsService.StartAsync(cancellationToken);
            //await _fetchAnnotationsService.StopAsync(cancellationToken);
            var test = await _recommendationsService.CalculateRecommendation("27658825", "MED");
            return new OkObjectResult("Done");
        }
    }
}
