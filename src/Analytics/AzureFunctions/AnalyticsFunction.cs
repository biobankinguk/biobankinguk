using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Analytics.Services.Contracts;
using Analytics.Services.Dto;
using System.Linq;

namespace Analytics.AnalyticsAzureFunctions
{
    public class AnalyticsFunction
    {
        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;

        public AnalyticsFunction(IAnalyticsReportGenerator analyticsReportGenerator)
        {
            _analyticsReportGenerator = analyticsReportGenerator;
        }

        [FunctionName("AnalyticsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetAnalyticsReport/{biobankId}/{year}/{endQuarter}/{reportPeriod}")] HttpRequest req,
            string biobankId,
            int year,
            int endQuarter,
            int reportPeriod,
            ILogger log)
        {
            log.LogInformation($"Fetching publications for {biobankId}");

            //Call GetBiobankReport method from service layer and load into DTO
            OrganisationAnalyticReportDTO report = await _analyticsReportGenerator.GetBiobankReport(biobankId,year,endQuarter,reportPeriod);

            log.LogInformation($"Organisation report generated");
            return new OkObjectResult(report);
        }
    }
}
