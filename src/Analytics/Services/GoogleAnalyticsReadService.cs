using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;

using Analytics.Services.Dto;
using Analytics.Services.Contracts;
using Analytics.Data.Repositories;
using Analytics.Data.Entities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Analytics.Services.Helpers;
using System.Security.Cryptography.X509Certificates;

namespace Analytics.Services
{
    // https://developers.google.com/analytics/devguides/reporting/core/v4/authorization
    public class GoogleAnalyticsReadService : IHostedService, IGoogleAnalyticsReadService
    {
        private readonly string VIEW_ID;
        private readonly string DATE_FORMAT = "yyyy-MM-dd";
        private readonly string START_DATE = "2016-01-01"; //specified in DATE_FORMAT

        private readonly GoogleCredential credentials;
        private readonly AnalyticsReportingService analytics;
        private readonly ILogger<GoogleAnalyticsReadService> _logger;
        private readonly IBiobankWebService _biobankWebService;
        private readonly IGenericEFRepository<OrganisationAnalytic> _organisationAnalyticRepository;
        private readonly IGenericEFRepository<DirectoryAnalyticEvent> _directoryAnalyticEventRepository;
        private readonly IGenericEFRepository<DirectoryAnalyticMetric> _directoryAnalyticMetricRepository;

        public GoogleAnalyticsReadService(IGenericEFRepository<OrganisationAnalytic> organisationAnalyticRepository,
                                          IGenericEFRepository<DirectoryAnalyticEvent> directoryAnalyticEventRepository,
                                          IGenericEFRepository<DirectoryAnalyticMetric> directoryAnalyticMetricRepository,
                                          IBiobankWebService biobankWebService,
                                          ILogger<GoogleAnalyticsReadService> logger,
                                          IConfiguration configuration)

        {
            var apikey   = Environment.GetEnvironmentVariable("analytics-apikey");
            this.VIEW_ID = Environment.GetEnvironmentVariable("analytics-viewid");

            this.credentials = GoogleCredential.FromJson(apikey)
                .CreateScoped(new[] { AnalyticsReportingService.Scope.AnalyticsReadonly });

            this.analytics = new AnalyticsReportingService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = this.credentials,
                    ApplicationName = "Google Analytics API v4 Biobanks"
                });
            this._organisationAnalyticRepository = organisationAnalyticRepository;
            this._directoryAnalyticEventRepository = directoryAnalyticEventRepository;
            this._directoryAnalyticMetricRepository = directoryAnalyticMetricRepository;
            this._biobankWebService = biobankWebService;
            this._logger = logger;
        }

        #region GoogleAnalytics API - Data Download

        public IList<Dimension> GetBiobankDimensions()
        {
            return new[] {
                new Dimension { Name = "ga:date" },
                new Dimension { Name = "ga:pagePath" },
                new Dimension { Name = "ga:previousPagePath" },
                new Dimension { Name = "ga:segment" },
                new Dimension { Name = "ga:source" },
                new Dimension { Name = "ga:hostname" },
                new Dimension { Name = "ga:city" }
            };
        }

        public IList<Metric> GetBiobankMetrics()
        {
            return new[] {
                new Metric { Expression = "ga:pageviews", Alias = "PageViews" }
            };
        }

        public IList<Dimension> GetDirectoryEventDimensions()
        {
            return new[] {
                new Dimension { Name = "ga:date" },
                new Dimension { Name = "ga:eventCategory" },
                new Dimension { Name = "ga:eventAction" },
                new Dimension { Name = "ga:eventLabel" },
                new Dimension { Name = "ga:segment" },
                new Dimension { Name = "ga:source" },
                new Dimension { Name = "ga:hostname" },
                new Dimension { Name = "ga:city" }
            };
        }

        public IList<Metric> GetDirectoryEventMetrics()
        {
            return new[] {
                new Metric { Expression = "ga:totalEvents"}
            };
        }

        public IList<Dimension> GetDirectoryMetricDimensions()
        {
            return new[] {
                new Dimension { Name = "ga:date" },
                new Dimension { Name = "ga:pagePath" },
                new Dimension { Name = "ga:pagePathLevel1" },
                new Dimension { Name = "ga:segment" },
                new Dimension { Name = "ga:source" },
                new Dimension { Name = "ga:hostname" },
                new Dimension { Name = "ga:city" }
            };
        }

        public IList<Metric> GetDirectoryMetricMetrics()
        {
            return new[] {
                new Metric { Expression = "ga:sessions"},
                new Metric { Expression = "ga:bouncerate"},
                new Metric { Expression = "ga:percentNewSessions"},
                new Metric { Expression = "ga:avgSessionDuration"},
            };
        }

        public IList<DimensionFilterClause> GetBiobankDimensionFilters(string biobankId)
        {
            var biobankFilter = BiobankDimensionFilter(new[] { biobankId });
            return ConstructDimensionFilter(biobankFilter);

        }

        public IList<ReportRequest> ConstructRequest(IList<Metric> metrics, IList<Dimension> dimensions, IList<Segment> segments, IList<DateRange> dateRanges,
                                              IList<DimensionFilterClause> dimensionfilterclauses = null, int pagesize = 100000)
        {
            var request = new ReportRequest
            {
                ViewId = VIEW_ID,
                DateRanges = dateRanges,
                Dimensions = dimensions,
                Metrics = metrics,
                Segments = segments,
                PageSize = pagesize
            };

            if (dimensionfilterclauses != null)
                request.DimensionFilterClauses = dimensionfilterclauses;

            return new[] { request };
        }

        public IList<DimensionFilter> BiobankDimensionFilter(IList<string> biobankIDs)
        {
            return new[]
            {
                new DimensionFilter
                {
                    DimensionName = "ga:pagePath",
                    Operator__ = "PARTIAL",
                    Expressions = biobankIDs
                }
            };
        }

        public IList<DimensionFilterClause> ConstructDimensionFilter(IList<DimensionFilter> filters, string filterOperator = "AND")
        {
            return new[]
            {
                new DimensionFilterClause
                {
                    Operator__ = filterOperator,
                    Filters = filters
                }
            };
        }

        public (string, IList<SegmentFilterClause>) NottLoughSegmentClause()
        {
            var name = "Sessions excluding Nottingham and Loughborough";
            var clause = new[] {
                new SegmentFilterClause {
                    Not = true,
                    DimensionFilter = new SegmentDimensionFilter {
                        DimensionName = "ga:city",
                        Operator__ = "IN_LIST",
                        Expressions = new[] {"Nottingham", "Loughborough"}
                    }
                }
            };

            return (name, clause);
        }

        public IList<Segment> ConstructSegment(string name, IList<SegmentFilterClause> segmentFilterClauses)
        {
            return new[] { new Segment {
                DynamicSegment = new DynamicSegment{
                    Name = name,
                    SessionSegment = new SegmentDefinition {
                        SegmentFilters = new[] {
                            new SegmentFilter {
                                SimpleSegment = new SimpleSegment {
                                    OrFiltersForSegment = new[] {
                                        new OrFiltersForSegment{
                                            SegmentFilterClauses = segmentFilterClauses
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
            }};
        }

        public IList<Segment> GetNottLoughSegment()
        {
            (var name, var clause) = NottLoughSegmentClause();
            return ConstructSegment(name, clause);
        }

        public GetReportsResponse GetReports(IList<ReportRequest> reportRequests)
        {
            var request = analytics.Reports.BatchGet(new GetReportsRequest
            {
                ReportRequests = reportRequests
            });

            return request.Execute();
        }

        //or use function overloading or dynamic?
        public async Task<DateTimeOffset> GetLatestBiobankEntry()
            => (await _organisationAnalyticRepository.ListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTimeOffset.MinValue).Max();

        public async Task<DateTimeOffset> GetLatestEventEntry()
            => (await _directoryAnalyticEventRepository.ListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTimeOffset.MinValue).Max();

        public async Task<DateTimeOffset> GetLatestMetricEntry()
            => (await _directoryAnalyticMetricRepository.ListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTimeOffset.MinValue).Max();


        public DateTimeOffset ConvertToDateTime(string inputDateTime, string format)
        {
            var status = DateTimeOffset.TryParseExact(inputDateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset outputDateTime);

            if (status)
                return outputDateTime;
            else //outputDateTime is MinValue by default anyways, if statement only for good measures
                return DateTimeOffset.MinValue;
        }

        public async Task DownloadBiobankDataById(string biobankId, IList<DateRange> dateRanges)
        {
            var metrics = GetBiobankMetrics();
            var dimensions = GetBiobankDimensions();
            var segments = GetNottLoughSegment();
            var dimensionFilters = GetBiobankDimensionFilters(biobankId);
            var reportRequest = ConstructRequest(metrics, dimensions, segments, dateRanges, dimensionFilters);

            var reportResponse = GetReports(reportRequest);
            var biobankData = reportResponse.Reports[0].Data.Rows;

            foreach (ReportRow bbd in biobankData)
            {
                _organisationAnalyticRepository.Insert(new OrganisationAnalytic
                {
                    Date = ConvertToDateTime(bbd.Dimensions[0], "yyyyMMdd"),
                    PagePath = bbd.Dimensions[1],
                    PreviousPagePath = bbd.Dimensions[2],
                    Segment = bbd.Dimensions[3],
                    Source = bbd.Dimensions[4],
                    Hostname = bbd.Dimensions[5],
                    City = bbd.Dimensions[6],
                    Counts = int.Parse(bbd.Metrics[0].Values[0]),
                    OrganisationExternalId = biobankId
                });
            }
            await _organisationAnalyticRepository.SaveChangesAsync();
        }

        public async Task DownloadAllBiobankData(IList<DateRange> dateRanges)
        {
            var metrics = GetBiobankMetrics();
            var dimensions = GetBiobankDimensions();
            var segments = GetNottLoughSegment();

            var biobanks = await _biobankWebService.GetOrganisationExternalIds();

            foreach (var biobankId in biobanks)
            { //TODO: replicated as in biobank.analytics python script but should be re-written to compile all requests into one list and run GetReports once
                var dimensionFilters = GetBiobankDimensionFilters(biobankId);
                var reportRequest = ConstructRequest(metrics, dimensions, segments, dateRanges, dimensionFilters);

                var reportResponse = GetReports(reportRequest);
                var biobankData = reportResponse.Reports[0].Data.Rows;

                foreach (ReportRow bbd in biobankData)
                {
                    _organisationAnalyticRepository.Insert(new OrganisationAnalytic
                    {
                        Date = ConvertToDateTime(bbd.Dimensions[0], "yyyyMMdd"),
                        PagePath = bbd.Dimensions[1],
                        PreviousPagePath = bbd.Dimensions[2],
                        Segment = bbd.Dimensions[3],
                        Source = bbd.Dimensions[4],
                        Hostname = bbd.Dimensions[5],
                        City = bbd.Dimensions[6],
                        Counts = int.Parse(bbd.Metrics[0].Values[0]),
                        OrganisationExternalId = biobankId
                    });
                }
                await _organisationAnalyticRepository.SaveChangesAsync();
                _logger.LogInformation($"Fetched analytics for data for {biobankId}");
            }
        }

        public async Task DownloadDirectoryData(IList<DateRange> dateRanges)
        {
            var metricMetrics = GetDirectoryMetricMetrics();
            var eventMetrics = GetDirectoryEventMetrics();
            var eventDimensions = GetDirectoryEventDimensions();
            var metricDimensions = GetDirectoryMetricDimensions();
            var segments = GetNottLoughSegment();

            var eventRequest = ConstructRequest(eventMetrics, eventDimensions, segments, dateRanges);
            var metricRequest = ConstructRequest(metricMetrics, metricDimensions, segments, dateRanges);

            var reportResponse = GetReports(new List<ReportRequest> { eventRequest[0], metricRequest[0] });
            var eventData = reportResponse.Reports[0].Data.Rows;
            var metricData = reportResponse.Reports[1].Data.Rows;

            foreach (ReportRow events in eventData)
            {
                _directoryAnalyticEventRepository.Insert(new DirectoryAnalyticEvent
                {
                    Date = ConvertToDateTime(events.Dimensions[0], "yyyyMMdd"),
                    EventCategory = events.Dimensions[1],
                    EventAction = events.Dimensions[2],
                    Biobank = events.Dimensions[3],
                    Segment = events.Dimensions[4],
                    Source = events.Dimensions[5],
                    Hostname = events.Dimensions[6],
                    City = events.Dimensions[7],
                    Counts = int.Parse(events.Metrics[0].Values[0]),
                });
            }
            await _directoryAnalyticEventRepository.SaveChangesAsync();

            foreach (ReportRow metric in metricData)
            {
                _directoryAnalyticMetricRepository.Insert(new DirectoryAnalyticMetric
                {
                    Date = ConvertToDateTime(metric.Dimensions[0], "yyyyMMdd"),
                    PagePath = metric.Dimensions[1],
                    PagePathLevel1 = metric.Dimensions[2],
                    Segment = metric.Dimensions[3],
                    Source = metric.Dimensions[4],
                    Hostname = metric.Dimensions[5],
                    City = metric.Dimensions[6],
                    Sessions = Convert.ToInt32(double.Parse(metric.Metrics[0].Values[0])),
                    BounceRate = Convert.ToInt32(double.Parse(metric.Metrics[0].Values[1])),
                    PercentNewSessions = Convert.ToInt32(double.Parse(metric.Metrics[0].Values[2])),
                    AvgSessionDuration = Convert.ToInt32(double.Parse(metric.Metrics[0].Values[3])),
                });
            }
            await _directoryAnalyticMetricRepository.SaveChangesAsync();
            _logger.LogInformation($"Fetched event and metric data for analytics");
        }

        #endregion

        public DateRange GetRelevantPeriod(int year, int endQuarter, int reportPeriod)
        {
            int monthsPerQuarter = 3;
            int month = endQuarter * monthsPerQuarter;
            int lastDayofQuarter = DateTime.DaysInMonth(year, month);

            DateTimeOffset endDate = new DateTimeOffset(year, month, lastDayofQuarter,0,0,0,TimeSpan.Zero);
            //get start date by subtracting report period (specified in quarters) from end date
            var startDate = endDate.AddMonths(-reportPeriod * monthsPerQuarter);

            return new DateRange { StartDate = startDate.ToString(DATE_FORMAT), EndDate = endDate.ToString(DATE_FORMAT) };
        }


        public async Task<IEnumerable<OrganisationAnalytic>> GetAllBiobankData()
            => (await _organisationAnalyticRepository.ListAsync());

        public async Task<IEnumerable<OrganisationAnalytic>> GetAllBiobankData(DateRange dateRange)
        {
            var startDate = DateTimeOffset.Parse(dateRange.StartDate);
            var endDate = DateTimeOffset.Parse(dateRange.EndDate);

            return await _organisationAnalyticRepository.ListAsync(
                false,
                x => x.Date >= startDate &&
                x.Date <= endDate);
        }

        public async Task<IEnumerable<DirectoryAnalyticEvent>> GetDirectoryEventData(DateRange dateRange)
        {
            var startDate = DateTimeOffset.Parse(dateRange.StartDate);
            var endDate = DateTimeOffset.Parse(dateRange.EndDate);

            return await _directoryAnalyticEventRepository.ListAsync(
                false,
                x => x.Date >= startDate &&
                x.Date <= endDate);
        }

        public async Task<IEnumerable<DirectoryAnalyticMetric>> GetDirectoryMetricData(DateRange dateRange)
        {
            var startDate = DateTimeOffset.Parse(dateRange.StartDate);
            var endDate = DateTimeOffset.Parse(dateRange.EndDate);

            return await _directoryAnalyticMetricRepository.ListAsync(
                false,
                x => x.Date >= startDate &&
                x.Date <= endDate);
        }

        public async Task<IEnumerable<OrganisationAnalytic>> GetBiobankDataById(string biobankId)
            => (await _organisationAnalyticRepository.ListAsync(false, x => x.OrganisationExternalId == biobankId));

        public async Task<IEnumerable<OrganisationAnalytic>> GetBiobankDataById(string biobankId, DateRange dateRange)
        {
            var startDate = DateTimeOffset.Parse(dateRange.StartDate);
            var endDate = DateTimeOffset.Parse(dateRange.EndDate);

            return await _organisationAnalyticRepository.ListAsync(
                false,
                x => x.OrganisationExternalId == biobankId &&
                x.Date >= startDate &&
                x.Date <= endDate);
        }

        public IEnumerable<OrganisationAnalytic> FilterByPagePath(IEnumerable<OrganisationAnalytic> biobankData, string path)
            => biobankData.Where(x => x.PagePath.Contains(path));//.ToList();

        public IEnumerable<DirectoryAnalyticMetric> FilterByPagePath(IEnumerable<DirectoryAnalyticMetric> metricData, string path)
            => metricData.Where(x => x.PagePath.Contains(path));//.ToList();

        public IEnumerable<OrganisationAnalytic> FilterByHost(IEnumerable<OrganisationAnalytic> biobankData)
            => biobankData.Where(x => x.Hostname.Contains("directory.biobankinguk.org"));

        public IEnumerable<DirectoryAnalyticEvent> FilterByHost(IEnumerable<DirectoryAnalyticEvent> eventData)
            => eventData.Where(x => x.Hostname.Contains("directory.biobankinguk.org"));

        public IEnumerable<DirectoryAnalyticMetric> FilterByHost(IEnumerable<DirectoryAnalyticMetric> metricData)
            => metricData.Where(x => x.Hostname.Contains("directory.biobankinguk.org"));

        public IEnumerable<DirectoryAnalyticEvent> FilterByEvent(IEnumerable<DirectoryAnalyticEvent> eventData, string strEvent)
            => eventData.Where(x => x.EventAction == strEvent);

        public string GetQuarter(DateTimeOffset date)
            => date.Year + "Q" + ((date.Month + 2) / 3);


        public IEnumerable<QuarterlySummary> GetSummary(IEnumerable<OrganisationAnalytic> biobankData)
            => biobankData.GroupBy(
                x => new { bb = x.OrganisationExternalId, q = GetQuarter(x.Date) },
                x => x.Counts,
                (key, group) => new QuarterlySummary
                {
                    Biobank = key.bb,
                    Quarter = key.q,
                    Count = group.Sum(),
                });

        public IEnumerable<QuarterlySummary> GetSummary(IEnumerable<DirectoryAnalyticEvent> eventData)
            => eventData.GroupBy(
                x => new { bb = x.Biobank, q = GetQuarter(x.Date) },
                x => x.Counts,
                (key, group) => new QuarterlySummary
                {
                    Biobank = key.bb,
                    Quarter = key.q,
                    Count = group.Sum(),
                });

        public IEnumerable<QuarterlySummary> GetSummary(IEnumerable<DirectoryAnalyticMetric> metricData, Func<DirectoryAnalyticMetric, int> elementSelector)
            => metricData.GroupBy(
                x => GetQuarter(x.Date),
                elementSelector,
                (key, group) => new QuarterlySummary
                {
                    Biobank = "Directory",
                    Quarter = key,
                    Count = group.Sum(),
                }).OrderBy(x=>x.Quarter);

        public IEnumerable<QuarterlySummary> GetRankings(IEnumerable<QuarterlySummary> summary)
            => summary.GroupBy(
                    x => x.Biobank,
                    x => x.Count,
                    (key, group) => new QuarterlySummary
                    {
                        Biobank = key,
                        Quarter = "Total",
                        Total = group.Sum()
                    }).OrderByDescending(y => y.Total);

        public (IList<string>, IList<QuarterlyCountsDto>) GetTopBiobanks(IEnumerable<QuarterlySummary> summary,
                                        IEnumerable<QuarterlySummary> ranking, string biobankId, int numOfTopBiobanks)
        {
            //Get Top Biobanks
            var getTopBiobanks = ranking.Take(numOfTopBiobanks);

            //if biobank isnt in top biobanks, append to list
            if (getTopBiobanks.Where(x => x.Biobank == biobankId).Count() == 0)
            {
                var bbRanking = ranking.Where(x => x.Biobank == biobankId);
                if (bbRanking.Count() > 0)
                    getTopBiobanks.Append(bbRanking.First());
                else
                    getTopBiobanks = getTopBiobanks.Append(new QuarterlySummary
                    {
                        Biobank = biobankId,
                        Quarter = "Total",
                        Total = 0,
                        Count = 0
                    });
            }

            // build table
            List<QuarterlyCountsDto> profileQuarters = new List<QuarterlyCountsDto>();
            var quarterLabels = summary.GroupBy(x => x.Quarter).OrderBy(x => x.Key).Select(x => x.Key).ToList();

            foreach (var bb in getTopBiobanks) //loop through top few biobanks
            {
                List<int> qcount = new List<int>();
                var quarterCount = summary.Where(x => x.Biobank == bb.Biobank)
                    .OrderBy(y => y.Quarter).Select(z => new { z.Count, z.Quarter });

                foreach (var ql in quarterLabels) //done using a loop to fill in '0's for quarters with no data
                    qcount.Add(quarterCount.Where(x => x.Quarter == ql).Select(x => x.Count).FirstOrDefault());


                profileQuarters.Add(new QuarterlyCountsDto
                {
                    BiobankId = bb.Biobank,
                    Total = bb.Total,
                    QuarterCount = qcount
                });
            }

            return (quarterLabels, profileQuarters);
        }

        public (IList<int>, IList<double>) GetQuarterlyAverages(IEnumerable<QuarterlySummary> summary, string biobankId)
        {
            List<double> bbAvg = new List<double>();
            List<int> bbVal = new List<int>();

            var bbCount = Convert.ToDouble(summary.GroupBy(x => x.Biobank).Select(x => x.Key).Count());

            var quarterLabels = summary.GroupBy(x => x.Quarter).OrderBy(x => x.Key).Select(x => x.Key).ToList();
            var quarterCount = summary.Where(x => x.Biobank == biobankId)
                    .OrderBy(y => y.Quarter).Select(z => new { z.Quarter, z.Count });

            foreach (var ql in quarterLabels)
            {   //done using a loop to fill in '0's for quarters with no data
                //Sum/count used insted of Average() for same reason
                bbAvg.Add(summary.Where(x => x.Quarter == ql).Select(x => x.Count).DefaultIfEmpty().Sum() / bbCount);
                bbVal.Add(quarterCount.Where(x => x.Quarter == ql).Select(x => x.Count).FirstOrDefault());
            }

            return (bbVal, bbAvg); //or maybe IList<(int,double)>?
        }

        public string GetViewRoute(string pagePath)
        {
            if (pagePath.Contains("/Search/Collection"))
                return "Search Existing Samples Query";
            else if (pagePath.Contains("/Search/Capabilities"))
                return "Require Samples Collected Query";
            else if (pagePath.Contains("/Profile/Biobank/"))
                return "UKCRC-TDCC Biobanks A-Z";
            else if (pagePath.Contains("/biobanks-a-z/"))
                return "UKCRC-TDCC Biobanks A-Z";
            else if (pagePath.Contains("/Profile/Network"))
                return "Biobank Network Profile Pages";
            else if ((pagePath.Contains("/Biobank/") && !pagePath.Contains("/Profile/Biobank/"))
                    || pagePath.Contains("/ADAC/") || pagePath.Contains("/Account/") || pagePath.Contains("/Register/"))
                return "Account & Backend Management";
            else
                return "Other";
        }

        public (IList<string>, IList<int>) GetPageRoutes(IEnumerable<OrganisationAnalytic> biobankData)
        {
            var query = biobankData.GroupBy(
                x => GetViewRoute(x.PreviousPagePath),
                x => x.Counts,
                (key, group) => (key, group.Sum())).OrderByDescending(x => x.Item2).ToList();

            return (query.Select(x => x.key).ToList(), query.Select(x => x.Item2).ToList());
        }

        public string GetSearchType(string pagePath)
        {
            if (pagePath.Contains("diagnosis="))
                return "Diagnosis";
            else if (pagePath.Contains("selectedFacets="))
                return "Filters";
            else
                return "No specific search";
        }

        public string GetSearchTerm(string pagePath)
        {
            if (pagePath.Contains("diagnosis="))
                return pagePath.Split(new[] { "diagnosis=" }, StringSplitOptions.None)[1]
                    .Split('&').FirstOrDefault();
            else if (pagePath.Contains("selectedFacets="))
                return "Filter";
            else
                return "Other";
        }

        public string[] GetSearchFilters(string pagePath)
        {
            if (pagePath.Contains("selectedFacets="))
                return pagePath.Split(new[] { "selectedFacets=" }, StringSplitOptions.None)[1]
                    .Replace('"', ' ').Replace('[', ' ').Replace(']', ' ').Split(',');
            else
                return new[] { "" };
        }

        public (IList<string>, IList<int>) GetSearchBreakdown(IEnumerable<OrganisationAnalytic> biobankData, Func<string, string> getSearchFunc)
        {
            var query = biobankData.GroupBy(
                x => getSearchFunc(x.PagePath),
                x => x.Counts,
                (key, group) => (key, group.Sum())).OrderByDescending(x => x.Item2).ToList();

            return (query.Select(x => x.key).ToList(), query.Select(x => x.Item2).ToList());
        }

        public (IList<string>, IList<int>) GetSearchFilters(IEnumerable<OrganisationAnalytic> biobankData)
        {
            List<string> filters = new List<string>();
            List<int> filterCount = new List<int>();

            foreach (var bb in biobankData)
            {
                var fterms = GetSearchFilters(bb.PagePath);
                foreach (var term in fterms)
                {
                    var sterm = term.Split('_').LastOrDefault().TrimStart(' ').TrimEnd(' ');
                    if (sterm == "")
                        continue;

                    if (!filters.Contains(sterm))
                    {

                        filters.Add(sterm);
                        filterCount.Add(1);
                    }
                    else
                    {
                        filterCount[filters.IndexOf(sterm)] += 1;
                    }
                }
            }
            return (filters, filterCount);

        }

        public IEnumerable<DirectoryAnalyticMetric> ApplySessionMulitplication(IEnumerable<DirectoryAnalyticMetric> metricData)
        {
            var multipliledMetricData = metricData.Select(x => new DirectoryAnalyticMetric
            {
                BounceRate = x.BounceRate * x.Sessions,
                AvgSessionDuration = x.AvgSessionDuration * x.Sessions,
                PercentNewSessions = x.PercentNewSessions * x.Sessions,

                City = x.City,
                Date = x.Date,
                Hostname = x.Hostname,
                Id = x.Id,
                PagePath = x.PagePath,
                PagePathLevel1 = x.PagePathLevel1,
                Segment = x.Segment,
                Sessions = x.Sessions,
                Source = x.Source

            });

            return multipliledMetricData;
        }

        public (IList<string>, IList<int>) GetSessionCount(IEnumerable<DirectoryAnalyticMetric> sessionData)
        {
            var sessionSummary = GetSummary(sessionData, x => x.Sessions);
            var quarterLabels = sessionSummary.Select(x => x.Quarter).ToList();
            var quarterCounts = sessionSummary.Select(x => x.Count).ToList();
            return (quarterLabels, quarterCounts);
        }

        public (IList<string>, IList<int>) GetContactCount(IEnumerable<DirectoryAnalyticEvent> eventData)
        {
            var eventSummary = eventData.GroupBy(
                x => GetQuarter(x.Date),
                x=> x.Counts,
                (key, group) => new QuarterlySummary
                {
                    Biobank = "Directory",
                    Quarter = key,
                    Count = group.Sum(),
                }).OrderBy(x => x.Quarter);
            var quarterLabels = eventSummary.Select(x => x.Quarter).ToList();
            var quarterCounts = eventSummary.Select(x => x.Count).ToList();
            return (quarterLabels, quarterCounts);
        }

        public (IList<string>, IList<double>) GetWeightedAverage(IEnumerable<DirectoryAnalyticMetric> sessionData, Func<DirectoryAnalyticMetric, int> elementSelector)
        {
            var sessionSummary = GetSummary(sessionData, x => x.Sessions);
            var elementSummary = GetSummary(sessionData, elementSelector);

            var quarterLabels = elementSummary.Select(x => x.Quarter).ToList();
            List<double> weightedAvg = new List<double>();

            foreach (var item in elementSummary)
            {
                var denom = Convert.ToDouble(sessionSummary.Where(x => x.Quarter == item.Quarter).Select(x => x.Count).FirstOrDefault());
                if (denom > 0)
                    weightedAvg.Add(item.Count / denom);
                else
                    weightedAvg.Add(item.Count);
            }
            return (quarterLabels, weightedAvg);
        }

        public (IList<string>, IList<int>) GetFilteredEventCount(IEnumerable<DirectoryAnalyticEvent> eventData, int threshold)
        {
            var eventsPerCityPerDay = eventData.GroupBy(
                x => new { city = x.City, date = x.Date.Date},
                x => x.Counts,
                (key, group) => new
                {
                    City = key.city,
                    Date = key.date,
                    Count = group.Sum(),
                });

            var summary = eventsPerCityPerDay
                .Where(x=>x.Count <= threshold).GroupBy(
                x => GetQuarter(x.Date),
                x => x.Count,
                (key, group) => new QuarterlySummary
                {
                    Biobank = "Directory",
                    Quarter = key,
                    Count = group.Sum(),
                }).OrderBy(x => x.Quarter);

            return (summary.Select(x => x.Quarter).ToList(), summary.Select(x => x.Count).ToList());
        }

        public IList<SourceCountDto> GetPageSources(IEnumerable<OrganisationAnalytic> biobankData, int numOfTopSources)
        {
            var totalCount = biobankData.Count();
            var sources = biobankData.GroupBy(
                x => x.Source,
                x => x.Counts,
                (key, group) => new SourceCountDto
                {
                    Source = key,
                    Count = group.Count(),
                    Percentage = (group.Count()/Convert.ToDouble(totalCount))*100
                }).OrderByDescending(x=>x.Count).Take(numOfTopSources);

            sources = sources.Append(new SourceCountDto
            {
                Source = "Others",
                Count = totalCount - sources.Select(x => x.Count).Sum(),
                Percentage = 100.0 - sources.Select(x => x.Percentage).Sum()
            });

            return sources.ToList();
        }

        //typically performed quatertly. Should be hit by scheduler
        public async Task UpdateAnalyticsData()
        {
            var lastBiobankEntry = await GetLatestBiobankEntry();
            var lastEventEntry = await GetLatestMetricEntry();
            var lastMetricEntry = await GetLatestEventEntry();

            //get most recent of all, assuming directory and biobank analytics data are always updated together
            //consider using myTimer.ScheduleStatus.LastUpdated from Azure function
            var lastentry = new[] { lastBiobankEntry, lastEventEntry, lastMetricEntry }.Max();

            // If no previous analtytics record
            if (lastentry == DateTimeOffset.MinValue)
            {
                var dateRange = new[] { new DateRange { StartDate = START_DATE, EndDate = DateTimeOffset.Now.ToString(DATE_FORMAT) } };
                await DownloadAllBiobankData(dateRange);
                await DownloadDirectoryData(dateRange);
            }
            // If last entry is in the past
            else if (lastentry > DateTimeOffset.MinValue && lastentry < DateTimeOffset.Now)
            {
                var dateRange = new[] { new DateRange { StartDate = lastentry.ToString(DATE_FORMAT), EndDate = DateTimeOffset.Now.ToString(DATE_FORMAT) } };
                await DownloadAllBiobankData(dateRange);
                await DownloadDirectoryData(dateRange);
            }
            // if data is up to date
            else
            {
                //do nothing
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Call directory for all active organisation
            var biobanks = await _biobankWebService.GetOrganisationNames();

            _logger.LogInformation($"Fetching analytics for {biobanks.Count()} organisations");

            // Fetch and store analytics data for each organisation
            await UpdateAnalyticsData();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


        //Test Functions
        public async Task SeedTestData()
        {
            var dateRange = new[] { new DateRange { StartDate = "2020-06-25", EndDate = DateTimeOffset.Now.ToString("2020-07-01") } };
            await DownloadAllBiobankData(dateRange);
            await DownloadDirectoryData(dateRange);
        }

    }
}
