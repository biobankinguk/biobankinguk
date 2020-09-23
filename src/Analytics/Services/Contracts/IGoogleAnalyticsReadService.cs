using Analytics.Data.Entities;
using Analytics.Services.Dto;
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using Analytics.Services.Helpers;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Analytics.Services.Contracts
{
    public interface IGoogleAnalyticsReadService : IHostedService
    {
        IList<DimensionFilter> BiobankDimensionFilter(IList<string> biobankIDs);
        IList<DimensionFilterClause> ConstructDimensionFilter(IList<DimensionFilter> filters, string filterOperator = "AND");
        IList<ReportRequest> ConstructRequest(IList<Metric> metrics, IList<Dimension> dimensions, IList<Segment> segments, IList<DateRange> dateRanges, IList<DimensionFilterClause> dimensionfilterclauses = null, int pagesize = 100000);
        IList<Segment> ConstructSegment(string name, IList<SegmentFilterClause> segmentFilterClauses);
        DateTimeOffset ConvertToDateTime(string inputDateTime, string format);
        Task DownloadAllBiobankData(IList<DateRange> dateRanges);
        Task DownloadBiobankDataById(string biobankId, IList<DateRange> dateRanges);
        Task DownloadDirectoryData(IList<DateRange> dateRanges);
        IEnumerable<DirectoryAnalyticEvent> FilterByEvent(IEnumerable<DirectoryAnalyticEvent> eventData, string strEvent);
        IEnumerable<DirectoryAnalyticEvent> FilterByHost(IEnumerable<DirectoryAnalyticEvent> eventData);
        IEnumerable<OrganisationAnalytic> FilterByHost(IEnumerable<OrganisationAnalytic> biobankData);
        IEnumerable<DirectoryAnalyticMetric> FilterByHost(IEnumerable<DirectoryAnalyticMetric> metricData);
        IEnumerable<OrganisationAnalytic> FilterByPagePath(IEnumerable<OrganisationAnalytic> biobankData, string path);
        IEnumerable<DirectoryAnalyticMetric> FilterByPagePath(IEnumerable<DirectoryAnalyticMetric> metricData, string path);
        Task<IEnumerable<OrganisationAnalytic>> GetAllBiobankData();
        Task<IEnumerable<OrganisationAnalytic>> GetAllBiobankData(DateRange dateRange);
        Task<IEnumerable<OrganisationAnalytic>> GetBiobankDataById(string biobankId);
        Task<IEnumerable<OrganisationAnalytic>> GetBiobankDataById(string biobankId, DateRange dateRange);
        IList<DimensionFilterClause> GetBiobankDimensionFilters(string biobankId);
        IList<Dimension> GetBiobankDimensions();
        IList<Metric> GetBiobankMetrics();
        Task<IEnumerable<DirectoryAnalyticEvent>> GetDirectoryEventData(DateRange dateRange);
        IList<Dimension> GetDirectoryEventDimensions();
        IList<Metric> GetDirectoryEventMetrics();
        Task<IEnumerable<DirectoryAnalyticMetric>> GetDirectoryMetricData(DateRange dateRange);
        IList<Dimension> GetDirectoryMetricDimensions();
        IList<Metric> GetDirectoryMetricMetrics();
        Task<DateTimeOffset> GetLatestBiobankEntry();
        Task<DateTimeOffset> GetLatestEventEntry();
        Task<DateTimeOffset> GetLatestMetricEntry();
        IList<Segment> GetNottLoughSegment();
        (IList<string>, IList<int>) GetPageRoutes(IEnumerable<OrganisationAnalytic> biobankData);
        string GetQuarter(DateTimeOffset date);
        (IList<int>, IList<double>) GetQuarterlyAverages(IEnumerable<QuarterlySummary> summary, string biobankId);
        IEnumerable<QuarterlySummary> GetRankings(IEnumerable<QuarterlySummary> summary);
        DateRange GetRelevantPeriod(int year, int endQuarter, int reportPeriod);
        GetReportsResponse GetReports(IList<ReportRequest> reportRequests);
        (IList<string>, IList<int>) GetSearchBreakdown(IEnumerable<OrganisationAnalytic> biobankData, Func<string, string> getSearchFunc);
        (List<string>, List<int>) GetSearchFilters(IEnumerable<OrganisationAnalytic> biobankData);
        string[] GetSearchFilters(string pagePath);
        string GetSearchTerm(string pagePath);
        string GetSearchType(string pagePath);
        IEnumerable<QuarterlySummary> GetSummary(IEnumerable<DirectoryAnalyticEvent> eventData);
        IEnumerable<QuarterlySummary> GetSummary(IEnumerable<OrganisationAnalytic> biobankData);
        IEnumerable<QuarterlySummary> GetSummary(IEnumerable<DirectoryAnalyticMetric> metricData, Func<DirectoryAnalyticMetric, int> elementSelector);
        (IList<string>, IList<QuarterlyCountsDto>) GetTopBiobanks(IEnumerable<QuarterlySummary> summary, IEnumerable<QuarterlySummary> ranking, string biobankId, int numOfTopBiobanks);
        string GetViewRoute(string pagePath);
        (string, IList<SegmentFilterClause>) NottLoughSegmentClause();
        Task UpdateAnalyticsData();
        IEnumerable<DirectoryAnalyticMetric> ApplySessionMulitplication(IEnumerable<DirectoryAnalyticMetric> metricData);
        (List<string>, List<int>) GetSessionNumber(IEnumerable<DirectoryAnalyticMetric> sessionData);
        (List<string>, List<double>) GetWeightedAverage(IEnumerable<DirectoryAnalyticMetric> sessionData, Func<DirectoryAnalyticMetric, int> elementSelector);

        //REMOVE TEST FUNC
        Task SeedTestData();
    }
}