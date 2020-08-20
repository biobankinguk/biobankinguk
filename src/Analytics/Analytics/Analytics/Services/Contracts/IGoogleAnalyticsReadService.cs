using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Services
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
        IList<DimensionFilterClause> GetBiobankDimensionFilters(string biobankId);
        IList<Dimension> GetBiobankDimensions();
        IList<Metric> GetBiobankMetrics();
        IList<Dimension> GetDirectoryEventDimensions();
        IList<Metric> GetDirectoryEventMetrics();
        IList<Dimension> GetDirectoryMetricDimensions();
        IList<Metric> GetDirectoryMetricMetrics();
        Task<DateTime> GetLatestBiobankEntry();
        Task<DateTime> GetLatestEventEntry();
        Task<DateTime> GetLatestMetricEntry();
        IList<Segment> GetNottLoughSegment();
        GetReportsResponse GetReports(IList<ReportRequest> reportRequests);
        (string, IList<SegmentFilterClause>) NottLoughSegmentClause();
        Task RunTest(string biobankId);
        Task SeedTestAnalyticsData();
        GetReportsResponse TestAPI();
        Task UpdateAnalyticsData();
    }
}