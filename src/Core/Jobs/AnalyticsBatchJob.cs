using Biobanks.Analytics.Core.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AnalyticsBatchJob
    {

        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsBatchJob(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        public async Task Run()
            => await _googleAnalyticsReadService.UpdateAnalyticsData();

        //[Function("AnalyticsFunction")]
        //public async Task<HttpResponseData> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetAnalyticsReport/{biobankId}/{year}/{endQuarter}/{reportPeriod}")] HttpRequestData req,
        //    string biobankId,
        //    int year,
        //    int endQuarter,
        //    int reportPeriod,
        //    FunctionContext functionContext)
        //{
        //    var logger = functionContext.GetLogger<AnalyticsFunction>();

        //    logger.LogInformation($"Fetching analytics for {biobankId}");

        //    //Call GetBiobankReport method from service layer and load into DTO
        //    var report = await _analyticsReportGenerator.GetBiobankReport(biobankId, year, endQuarter, reportPeriod);

        //    logger.LogInformation($"Organisation report generated");

        //    var response = req.CreateResponse(HttpStatusCode.OK);

        //    await response.WriteAsJsonAsync(report);

        //    return response;
        //}

        //[Function("DirectoryAnalyticsFunction")]
        //public async Task<HttpResponseData> Run(
        //[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetDirectoryAnalyticsReport/{year}/{endQuarter}/{reportPeriod}")] HttpRequestData req,
        //FunctionContext functionContext,
        //int year,
        //int endQuarter,
        //int reportPeriod)
        //    {
        //        var logger = functionContext.GetLogger<AnalyticsFunction>();

        //        logger.LogInformation($"Fetching analytics for tissue directory");

        //        //Call GetDirectoryAnalyticsReport method from service layer and load into DTO
        //        var report = await _analyticsReportGenerator.GetDirectoryReport(year, endQuarter, reportPeriod);

        //        logger.LogInformation($"Directory report generated");

        //        var response = req.CreateResponse(HttpStatusCode.OK);

        //        await response.WriteAsJsonAsync(report);

        //        return response;
        //    }


    }
}
