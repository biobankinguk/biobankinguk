using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Biobanks.Analytics.Core.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Biobanks.Analytics.AzFunctions
{
    public class AnalyticsFunction
    {
        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;

        public AnalyticsFunction(IAnalyticsReportGenerator analyticsReportGenerator)
        {
            _analyticsReportGenerator = analyticsReportGenerator;
        }

        [Function("AnalyticsFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetAnalyticsReport/{biobankId}/{year}/{endQuarter}/{reportPeriod}")] HttpRequestData req,
            string biobankId,
            int year,
            int endQuarter,
            int reportPeriod,
            FunctionContext functionContext)
        {
            var logger = functionContext.GetLogger<AnalyticsFunction>();

            logger.LogInformation($"Fetching analytics for {biobankId}");

            //Call GetBiobankReport method from service layer and load into DTO
            var report = await _analyticsReportGenerator.GetBiobankReport(biobankId, year, endQuarter, reportPeriod);

            logger.LogInformation($"Organisation report generated");

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(report);

            return response;
        }
    }
}
