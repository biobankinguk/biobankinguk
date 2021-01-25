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
using System.Threading;
using Publications.Services;
using Publications;
using System.Linq;

namespace PublicationsAzureFunctions
{
    public class Testing
    {
        private IEpmcService _epmcService;
        private FetchPublicationsService _fetchPublicationsService;
        private FetchAnnotationsService _fetchAnnotationsService;
        private readonly CancellationToken cancellationToken;

        public Testing(IEpmcService epmcService, FetchPublicationsService fetchPublicationsService, FetchAnnotationsService fetchAnnotationsService)
        {
            _epmcService = epmcService;
            _fetchPublicationsService = fetchPublicationsService;
            _fetchAnnotationsService = fetchAnnotationsService;
        }

        [FunctionName("Testing")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //var test = await _epmcService.GetPublicationAnnotations(126679, "MED");
            await _fetchAnnotationsService.StartAsync(cancellationToken);
            await _fetchAnnotationsService.StopAsync(cancellationToken);
            //var publicationId = 33340264;


            return new OkObjectResult("Done");
        }
    }
}
