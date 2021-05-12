using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AnalyticsJob
    {
        public AnalyticsJob()
        {
        }

        public async Task Run()
        {
            await Task.CompletedTask;
        }


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

        //[Function("BatchFunction")]
        ////Configured to run every quarter (At 00:00 on the 1st day in every 3rd month)
        //public async Task Run([TimerTrigger("0 0 0 1 */3 *", RunOnStartup = false)] TimerInfo myTimer, ILogger log) //remove runonstartup before deploying
        //{
        //    log.LogInformation($"C# Timer trigger function executed at: {DateTimeOffset.Now}");

        //    //Pulls Biobanks from directory (test), gets Analytics from API and pushes to Azure DB
        //    await _googleAnalyticsReadService.UpdateAnalyticsData();

        //    log.LogInformation($"C# Timer trigger function executed successfully");
        //}
    }
}
