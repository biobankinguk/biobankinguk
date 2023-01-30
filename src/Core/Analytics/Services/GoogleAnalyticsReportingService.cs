using Biobanks.Data;
using Biobanks.Entities.Data.Analytics;
using Biobanks.Shared.Services.Contracts;

using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Biobanks.Analytics.Services
{
    /// <inheritdoc />
    public class GoogleAnalyticsReportingService : IGoogleAnalyticsReportingService
    {
        private const string _gaDateRangeFormat = "yyyy-MM-dd";

        private readonly AnalyticsOptions _config;
        private readonly ApplicationDbContext _db;
        private readonly IOrganisationService _organisations;
        private readonly ILogger<GoogleAnalyticsReportingService> _logger;
        private readonly AnalyticsReportingService _analytics;

        public GoogleAnalyticsReportingService(
            ApplicationDbContext db,
            IOrganisationService organisations,
            IOptions<AnalyticsOptions> options,
            ILogger<GoogleAnalyticsReportingService> logger)
        {
            _config = options.Value;
            _db = db;
            _organisations = organisations;
            _logger = logger;

            var gaCredentials = GoogleCredential
                .FromJson(_config.GoogleAnalyticsReportingKey)
                .CreateScoped(new[]
                {
                    AnalyticsReportingService.Scope.AnalyticsReadonly
                });

            _analytics = new AnalyticsReportingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = gaCredentials,
                ApplicationName = "Google Analytics API v4 Biobanks"
            });
        }

        /// <summary>
        /// Get a Google AnalyticsReporting DateRange from
        /// DateTimeOffset values for start and end dates.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private DateRange DateRangeFromBounds(DateTimeOffset startDate, DateTimeOffset endDate)
            => new() { StartDate = startDate.ToString(_gaDateRangeFormat), EndDate = endDate.ToString(_gaDateRangeFormat) };

        /// <inheritdoc />
        public async Task DownloadAllBiobankData(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var dateRanges = new[] { DateRangeFromBounds(startDate, endDate) };

            var metrics = GetBiobankMetrics();
            var dimensions = GetBiobankDimensions();    
            var segments = GetNottLoughSegment();

            var biobanks = await _organisations.ListExternalIds();

            foreach (var biobankId in biobanks)
            { //TODO: replicated as in biobank.analytics python script but should be re-written to compile all requests into one list and run GetReports once
                var dimensionFilters = GetBiobankDimensionFilters(biobankId);
                var reportRequest = ConstructRequest(metrics, dimensions, segments, dateRanges, dimensionFilters);

                var reportResponse = GetReports(reportRequest);
                var biobankData = reportResponse.Reports[0].Data.Rows;

                if (biobankData != null)
                {
                    foreach (ReportRow bbd in biobankData)
                    {
                        _db.OrganisationAnalytics.Add(new OrganisationAnalytic
                        {
                            Date = ConvertToDateTime(bbd.Dimensions[0], "yyyyMMdd"),
                            PagePath = Regex.Replace(bbd.Dimensions[1], "(diagnosis=)", "ontologyTerm="),
                            PreviousPagePath = Regex.Replace(bbd.Dimensions[2], "(diagnosis=)", "ontologyTerm="),
                            Segment = bbd.Dimensions[3],
                            Source = bbd.Dimensions[4],
                            Hostname = bbd.Dimensions[5],
                            City = bbd.Dimensions[6],
                            Counts = int.Parse(bbd.Metrics[0].Values[0]),
                            OrganisationExternalId = biobankId
                        });
                    }
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"Fetched analytics for data for {biobankId}");
                }
                else
                    _logger.LogInformation($"biobankdata null for {biobankId}. Data not saved.");
            }
        }

        /// <inheritdoc />
        public async Task DownloadDirectoryData(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var dateRanges = new[] { DateRangeFromBounds(startDate, endDate) };

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

            if (eventData != null)
            {
                foreach (ReportRow events in eventData)
                {
                    _db.DirectoryAnalyticEvents.Add(new DirectoryAnalyticEvent
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
                await _db.SaveChangesAsync();
            }
            else
                _logger.LogInformation("eventData null. AnalyticEventData not added.");

            foreach (ReportRow metric in metricData)
            {
                _db.DirectoryAnalyticMetrics.Add(new DirectoryAnalyticMetric
                {
                    Date = ConvertToDateTime(metric.Dimensions[0], "yyyyMMdd"),
                    PagePath = Regex.Replace(metric.Dimensions[1], "(diagnosis=)", "ontologyTerm="),
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
            await _db.SaveChangesAsync();
            _logger.LogInformation("Fetched event and metric data for analytics");
        }

        #region Helpers

        private IList<Dimension> GetBiobankDimensions()
            => new[] {
                new Dimension { Name = "ga:date" },
                new Dimension { Name = "ga:pagePath" },
                new Dimension { Name = "ga:previousPagePath" },
                new Dimension { Name = "ga:segment" },
                new Dimension { Name = "ga:source" },
                new Dimension { Name = "ga:hostname" },
                new Dimension { Name = "ga:city" }
            };


        private IList<Metric> GetBiobankMetrics()
            => new[] {
                new Metric { Expression = "ga:pageviews", Alias = "PageViews" }
            };

        private IList<Dimension> GetDirectoryEventDimensions()
            => new[] {
                new Dimension { Name = "ga:date" },
                new Dimension { Name = "ga:eventCategory" },
                new Dimension { Name = "ga:eventAction" },
                new Dimension { Name = "ga:eventLabel" },
                new Dimension { Name = "ga:segment" },
                new Dimension { Name = "ga:source" },
                new Dimension { Name = "ga:hostname" },
                new Dimension { Name = "ga:city" }
            };

        private IList<Metric> GetDirectoryEventMetrics()
            => new[] {
                new Metric { Expression = "ga:totalEvents"}
            };

        private IList<Dimension> GetDirectoryMetricDimensions()
            => new[] {
                new Dimension { Name = "ga:date" },
                new Dimension { Name = "ga:pagePath" },
                new Dimension { Name = "ga:pagePathLevel1" },
                new Dimension { Name = "ga:segment" },
                new Dimension { Name = "ga:source" },
                new Dimension { Name = "ga:hostname" },
                new Dimension { Name = "ga:city" }
            };

        private IList<Metric> GetDirectoryMetricMetrics()
            => new[] {
                new Metric { Expression = "ga:sessions"},
                new Metric { Expression = "ga:bouncerate"},
                new Metric { Expression = "ga:percentNewSessions"},
                new Metric { Expression = "ga:avgSessionDuration"},
            };

        private IList<DimensionFilterClause> GetBiobankDimensionFilters(string biobankId)
            => ConstructDimensionFilter(BiobankDimensionFilter(new[] { biobankId }));

        private IList<ReportRequest> ConstructRequest(
            IList<Metric> metrics,
            IList<Dimension> dimensions,
            IList<Segment> segments,
            IList<DateRange> dateRanges,
            IList<DimensionFilterClause> dimensionfilterclauses = null,
            int pagesize = 100000)
        {
            var request = new ReportRequest
            {
                ViewId = _config.GoogleAnalyticsViewId,
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

        private IList<DimensionFilter> BiobankDimensionFilter(IList<string> biobankIDs)
            => new[]
            {
                new DimensionFilter
                {
                    DimensionName = "ga:pagePath",
                    Operator__ = "PARTIAL",
                    Expressions = biobankIDs
                }
            };

        private IList<DimensionFilterClause> ConstructDimensionFilter(IList<DimensionFilter> filters, string filterOperator = "AND")
            => new[]
            {
                new DimensionFilterClause
                {
                    Operator__ = filterOperator,
                    Filters = filters
                }
            };

        private (string, IList<SegmentFilterClause>) NottLoughSegmentClause()
            => ("Sessions excluding Nottingham and Loughborough",
                new[]
                {
                    new SegmentFilterClause
                    {
                        Not = true,
                        DimensionFilter = new SegmentDimensionFilter
                        {
                            DimensionName = "ga:city",
                            Operator__ = "IN_LIST",
                            Expressions = new[] {"Nottingham", "Loughborough"}
                        }
                    }
                });

        private IList<Segment> ConstructSegment(string name, IList<SegmentFilterClause> segmentFilterClauses)
            => new[] {
                new Segment
                {
                    DynamicSegment = new ()
                    {
                        Name = name,
                        SessionSegment = new ()
                        {
                            SegmentFilters = new[]
                            {
                                new SegmentFilter
                                {
                                    SimpleSegment = new ()
                                    {
                                        OrFiltersForSegment = new[] {
                                            new OrFiltersForSegment
                                            {
                                                SegmentFilterClauses = segmentFilterClauses
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

        private IList<Segment> GetNottLoughSegment()
        {
            (var name, var clause) = NottLoughSegmentClause();
            return ConstructSegment(name, clause);
        }

        private GetReportsResponse GetReports(IList<ReportRequest> reportRequests)
        {
            var request = _analytics.Reports.BatchGet(new GetReportsRequest
            {
                ReportRequests = reportRequests
            });

            return request.Execute();
        }

        private DateTimeOffset ConvertToDateTime(string inputDateTime, string format)
        {
            var status = DateTimeOffset.TryParseExact(inputDateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset outputDateTime);

            if (status)
                return outputDateTime;
            else //outputDateTime is MinValue by default anyways, if statement only for good measures
                return DateTimeOffset.MinValue;
        }

        #endregion
    }
}
