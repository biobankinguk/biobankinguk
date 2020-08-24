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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetAnalyticsReport?biobank={biobank}&year={year}&endQuarter={endQuarter}&reportPeriod={reportPeriod}")] HttpRequest req,
            string biobank,
            int year,
            int endQuarter,
            int reportPeriod,
            ILogger log)
        {
            log.LogInformation($"Fetching publications for {biobank}");

            //Call GetOrganisationPublications method from service layer and load into DTO
            BiobankAnalyticReportDTO report = await _analyticsReportGenerator.GetBiobankReport(biobank,year,endQuarter,reportPeriod);

            log.LogInformation($"Organisation report generated");
            return new OkObjectResult(report);
        }
    }
}
