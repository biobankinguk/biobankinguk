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

namespace PublicationsAzureFunctions
{
    public class PublicationFunction
    {
        private readonly IPublicationService _publication;

        public PublicationFunction(IPublicationService publication)
        {
            _publication = publication; 
        }
        [FunctionName("PublicationFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

        

            return new OkObjectResult($"Publications POST");
        
    }
}
