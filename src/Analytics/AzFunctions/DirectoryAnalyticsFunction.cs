using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Biobanks.Analytics.Core.Contracts;
using Microsoft.Azure.Functions.Worker;

namespace Biobanks.Analytics.AzFunctions
{
    public class DirectoryAnalyticsFunction
    {
        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;

        public DirectoryAnalyticsFunction(IAnalyticsReportGenerator analyticsReportGenerator)
        {
            _analyticsReportGenerator = analyticsReportGenerator;
        }

        [Function("DirectoryAnalyticsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetDirectoryAnalyticsReport/{year}/{endQuarter}/{reportPeriod}")] HttpRequest req,
            int year,
            int endQuarter,
            int reportPeriod,
            ILogger log)
        {
            log.LogInformation($"Fetching analytics for tissue directory");

            //Call GetDirectoryAnalyticsReport method from service layer and load into DTO
            var report = await _analyticsReportGenerator.GetDirectoryReport(year, endQuarter, reportPeriod);

            log.LogInformation($"Directory report generated");
            return new OkObjectResult(report);
        }
    }
}
