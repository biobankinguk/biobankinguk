using Analytics.Data.Entities;
using Analytics.Services.Dto;
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytics.Services.Contracts
{
    public interface IGoogleAnalyticsReadService
    {
        IList<DimensionFilter> BiobankDimensionFilter(IList<string> biobankIDs);
        IList<DimensionFilterClause> ConstructDimensionFilter(IList<DimensionFilter> filters, string filterOperator = "AND");
        IList<ReportRequest> ConstructRequest(IList<Metric> metrics, IList<Dimension> dimensions, IList<Segment> segments, IList<DateRange> dateRanges, IList<DimensionFilterClause> dimensionfilterclauses = null, int pagesize = 100000);
        IList<Segment> ConstructSegment(string name, IList<SegmentFilterClause> segmentFilterClauses);
        DateTime ConvertToDateTime(string inputDateTime, string format);
        Task DownloadAllBiobankData(IList<DateRange> dateRanges);
        Task DownloadBiobankDataById(string biobankId, IList<DateRange> dateRanges);
        Task DownloadDirectoryData(IList<DateRange> dateRanges);
        IEnumerable<DirectoryAnalyticEvent> FilterByEvent(IEnumerable<DirectoryAnalyticEvent> eventData, string strEvent);
        IEnumerable<DirectoryAnalyticEvent> FilterByHost(IEnumerable<DirectoryAnalyticEvent> eventData);
        IEnumerable<OrganisationAnalytic> FilterByHost(IEnumerable<OrganisationAnalytic> biobankData);
        IEnumerable<OrganisationAnalytic> FilterByPagePath(IEnumerable<OrganisationAnalytic> biobankData, string path);
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
        Task<DateTime> GetLatestBiobankEntry();
        Task<DateTime> GetLatestEventEntry();
        Task<DateTime> GetLatestMetricEntry();
        IList<Segment> GetNottLoughSegment();
        (IList<string>, IList<int>) GetPageRoutes(IEnumerable<OrganisationAnalytic> biobankData);
        string GetQuarter(DateTime date);
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
        (IList<string>, IList<QuarterlyCountsDTO>) GetTopBiobanks(IEnumerable<QuarterlySummary> summary, IEnumerable<QuarterlySummary> ranking, string biobankId, int numOfTopBiobanks);
        string GetViewRoute(string pagePath);
        (string, IList<SegmentFilterClause>) NottLoughSegmentClause();
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task UpdateAnalyticsData();
    }
}