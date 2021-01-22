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

namespace PublicationsAzureFunctions
{
    public class Testing
    {
        private IEpmcService _epmcService;
        private FetchPublicationsService _fetchPublicationsService;
        private readonly CancellationToken cancellationToken;

        public Testing(IEpmcService epmcService, FetchPublicationsService fetchPublicationsService)
        {
            _epmcService = epmcService;
            _fetchPublicationsService = fetchPublicationsService;
        }

        [FunctionName("Testing")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //var test = await _epmcService.GetAnnotationsByIdAndSource(126679, "MED");
            await _fetchPublicationsService.StartAsync(cancellationToken);
            await _fetchPublicationsService.StopAsync(cancellationToken);

            return new OkObjectResult("Done");
        }
    }
}
