using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Publications.Services;
using Publications.Services.Contracts;
using Publications;
using Microsoft.Extensions.Configuration;

namespace PublicationsAzureFunctions
{
    public class EPMCFunction
    {
        private readonly IEPMCService _epmc;
        public EPMCFunction(IEPMCService empc)
        {
            _epmc = empc;
        }

        [FunctionName("EPMCFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string biobank = "test";
            var publications  = _epmc.GetOrganisationPublications(biobank);

            return new OkObjectResult(publications);
        }
    }
}
