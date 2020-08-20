using System;
using System.Collections.Generic;

using Analytics.Repositories;
using Analytics.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;

namespace Analytics.Services
{
    // https://developers.google.com/analytics/devguides/reporting/core/v4/authorization
    public class GoogleAnalyticsReadService : IGoogleAnalyticsReadService
    {
        private const string VIEW_ID = "117031909"; //should be specified in config or ENV?
        private readonly string DATE_FORMAT = "yyyy-MM-dd";
        private readonly string START_DATE = "2016-01-01"; //specified in DATE_FORMAT

        private readonly GoogleCredential credentials;
        private readonly AnalyticsReportingService analytics;
        private readonly IBiobankReadService _biobankReadService;
        private readonly IGenericEFRepository<OrganisationAnalytic> _organisationAnalyticRepository;
        private readonly IGenericEFRepository<DirectoryAnalyticEvent> _directoryAnalyticEventRepository;
        private readonly IGenericEFRepository<DirectoryAnalyticMetric> _directoryAnalyticMetricRepository;
        
        //fix hardcoded string, use relative path pecified in config or ENV? 
        //also where to store api_key json file? 
        public GoogleAnalyticsReadService(IGenericEFRepository<OrganisationAnalytic> organisationAnalyticRepository,
                                          IGenericEFRepository<DirectoryAnalyticEvent> directoryAnalyticEventRepository,
                                          IGenericEFRepository<DirectoryAnalyticMetric> directoryAnalyticMetricRepository,
                                          IBiobankReadService biobankReadService,
                                          string apikeyfile = "C:\\Users\\Shakirudeen\\source\\repos\\biobankinguk\\src\\Analytics\\Analytics\\Services\\client_secret.json")
        {
            this.credentials = GoogleCredential.FromFile(apikeyfile)
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
            this._biobankReadService = biobankReadService;
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

        //or use function overloading to keep same name but pass in repository?
        public async Task<DateTime> GetLatestBiobankEntry()
            => (await _organisationAnalyticRepository.ListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTime.MinValue).Max();

        public async Task<DateTime> GetLatestEventEntry()
            => (await _directoryAnalyticEventRepository.ListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTime.MinValue).Max();

        public async Task<DateTime> GetLatestMetricEntry()
            => (await _directoryAnalyticMetricRepository.ListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTime.MinValue).Max();


        public DateTime ConvertToDateTime(string inputDateTime, string format)
        {
            var status = DateTime.TryParseExact(inputDateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime outputDateTime);

            if (status)
                return outputDateTime;
            else //outputDateTime is MinValue by default anyways, if statement only for good measures
                return DateTime.MinValue;
        }

        public async Task DownloadBiobankDataById(string biobankId, IList<DateRange> dateRanges)
        {
            var bb = await _biobankReadService.GetBiobankByExternalIdAsync(biobankId);
            var metrics = GetBiobankMetrics();
            var dimensions = GetBiobankDimensions();
            var segments = GetNottLoughSegment();
            var dimensionFilters = GetBiobankDimensionFilters(bb.OrganisationExternalId);
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

            var biobanks = await _biobankReadService.ListBiobanksAsync();

            foreach (Organisation bb in biobanks)
            { //should be re-written to compile all requests into one list and run GetReports once
                var dimensionFilters = GetBiobankDimensionFilters(bb.OrganisationExternalId);
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
                        OrganisationExternalId = bb.OrganisationExternalId
                    });
                }
                await _organisationAnalyticRepository.SaveChangesAsync();
                //Thread.Sleep(3000); 
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
        }

        //typically performed quatertly. Should be hit by scheduler
        public async Task UpdateAnalyticsData()
        {
            var lastBiobankEntry = await GetLatestBiobankEntry();
            var lastEventEntry = await GetLatestMetricEntry();
            var lastMetricEntry = await GetLatestEventEntry();

            //get most recent of all, assuming directory and biobank analytics data are always updated together
            //or seperate update functions can be written for each and then updated from respective latest entry.
            var lastentry = new[] { lastBiobankEntry, lastEventEntry, lastMetricEntry }.Max();

            // If no previous analtytics record
            if (lastentry == DateTime.MinValue)
            {
                var dateRange = new[] { new DateRange { StartDate = START_DATE, EndDate = DateTime.Now.ToString(DATE_FORMAT) } };
                await DownloadAllBiobankData(dateRange);
                await DownloadDirectoryData(dateRange);
            }
            // If last entry is in the past
            else if (lastentry > DateTime.MinValue && lastentry < DateTime.Now)
            {
                var dateRange = new[] { new DateRange { StartDate = lastentry.ToString(DATE_FORMAT), EndDate = DateTime.Now.ToString(DATE_FORMAT) } };
                await DownloadAllBiobankData(dateRange);
                await DownloadDirectoryData(dateRange);
            }
            // if data is up to date
            else
            {
                //do nothing
            }
        }

        #endregion

        //#######################TEST FUNCTIONS####################################

        public async Task SeedTestAnalyticsData()
        {

            //var dateRange = new[] { new DateRange { StartDate = "2020-06-25", EndDate = "2020-07-01" } }; //short test, small data
            var dateRange = new[] { new DateRange { StartDate = "2018-06-30", EndDate = "2020-07-31" } }; //big data
            await DownloadAllBiobankData(dateRange);
            await DownloadDirectoryData(dateRange);
        }

        public async Task RunTest(string biobankId)
            => await SeedTestAnalyticsData();


        //Simple method to test Analytics reporting API
        public GetReportsResponse TestAPI()
        {
            Console.WriteLine("Analytics API - Service Account");
            Console.WriteLine("==========================");
            GetReportsResponse response;

            using (analytics)
            {
                var request = analytics.Reports.BatchGet(new GetReportsRequest
                {
                    ReportRequests = new[] {
                        new ReportRequest{
                            DateRanges = new[] { new DateRange{ StartDate = "2020-06-25", EndDate = "2020-07-01" }},
                            Dimensions = new[] { new Dimension{ Name = "ga:date" }},
                            Metrics = new[] { new Metric{ Expression = "ga:sessions", Alias = "Sessions"}},
                            ViewId = "117031909"
                        }
                    }
                });
                response = request.Execute();
            }
            return response;
        }

    }
}
