using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Publications.Services.Contracts;
using System.Collections.Generic;
using Publications.Services;
using Publications;
using System.Linq;

namespace PublicationsAzureFunctions
{
    public class EpmcFunction
    {
        private IPublicationService _publicationService;

        public EpmcFunction(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        [FunctionName("EPMCFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetPublications/{biobank}")] HttpRequest req,
            string biobank,
            ILogger log)
        {
            log.LogInformation($"Fetching publications for {biobank}");

            //Call GetOrganisationPublications method from service layer and load into DTO
            IEnumerable<PublicationDto> publications = await _publicationService.GetOrganisationPublications(biobank);

            //Check if any publications were pulled from API
            if (publications?.Any() !=true)
            {
                log.LogInformation($"Publications for given biobank not found");
                return new BadRequestResult();
            }
            else
            {
                log.LogInformation($"Fetched and stored {publications.Count()} for {biobank}");
                return new OkObjectResult(publications);
            }
        }
    }
}
