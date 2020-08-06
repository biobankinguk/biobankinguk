using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Publications.Services.Contracts;
using System.Collections.Generic;
using Publications.Services;

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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetPublications/{biobank}")] 
        HttpRequest req,
            string biobank,
            ILogger log)
        {
            //Consume API and load into DTO
            Task<List<Publications.PublicationDTO>> publications = _epmc.GetOrganisationPublications(biobank);

            //Trigger next function (Publications service) and pass DTO 
            return new OkObjectResult($"Hello, {biobank}");
        }
    }
}
