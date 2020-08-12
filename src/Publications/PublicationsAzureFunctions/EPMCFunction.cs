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
        private IPublicationService _publication;
        public EPMCFunction(IEPMCService empc, IPublicationService publication)
        {
            _epmc = empc;
            _publication = publication;
        }

        [FunctionName("EPMCFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetPublications/{biobank}")] 
        HttpRequest req,
            string biobank,
            ILogger log)
        {
            log.LogInformation($"Fetching publications for {biobank}");

            //Call GetOrganisationPublications method from service layer and load into DTO
            var publications = await _epmc.GetOrganisationPublications(biobank);
            
            //Push DTO to Azure db
            await _publication.AddOrganisationPublications(biobank, publications);

            log.LogInformation($"Fetched and stored {publications.Count} for {biobank}");
            
            return new OkObjectResult($"Fetched and stored {publications.Count} for {biobank}");
            
        }
    }
}
