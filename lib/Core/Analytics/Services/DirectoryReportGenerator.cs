using Biobanks.Analytics.Dto;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Extensions;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Analytics;

namespace Biobanks.Analytics.Services
{
    public class DirectoryReportGenerator : IDirectoryReportGenerator
    {
        private readonly IGoogleAnalyticsReportingService _ga;
        private readonly IAnalyticsService _analytics;
        private readonly IReportDataTransformationService _transform;
        private readonly AnalyticsOptions _config;

        public DirectoryReportGenerator(
            IGoogleAnalyticsReportingService ga,
            IAnalyticsService analytics,
            IReportDataTransformationService transform,
            IOptions<AnalyticsOptions> options)
        {
            _ga = ga;
            _analytics = analytics;
            _transform = transform;
            _config = options.Value;
        }

        /// <inheritdoc />
        public async Task<DirectoryReportDto> GetReport(int year, int quarter, int period)
        {
            var (startDate, endDate) = _transform.PeriodAsDateRange(year, quarter, period);
            var biobankData = await _analytics.GetOrganisationAnalytics(startDate, endDate);
            var eventData = await _analytics.GetAnalyticsEvents(startDate, endDate);
            var metricData = await _analytics.GetAnalyticsMetrics(startDate, endDate);

            //filter by host
            if (!string.IsNullOrEmpty(_config.FilterHostname))
            {
                biobankData = biobankData.Where(x => x.Hostname.Contains(_config.FilterHostname));
                eventData = eventData.Where(x => x.Hostname.Contains(_config.FilterHostname));
                metricData = metricData.Where(x => x.Hostname.Contains(_config.FilterHostname));
            }

            var sessionStats = GetSessionStats(metricData);
            var sessionSearchStats = GetSessionSearchStats(metricData);
            var searchCharacteristics = GetSearchCharacteristics(biobankData);
            var eventStats = GetEventStats(eventData);
            var profilePageStats = GetProfilePageStats(biobankData);

            return new()
            {
                Year = year,
                EndQuarter = quarter,
                ReportPeriod = period,
                NumOfTopBiobanks = _config.MetricThreshold,
                EventsPerCityThreshold = _config.EventThreshold,

                SessionStats = sessionStats,
                SessionSearchStats = sessionSearchStats,
                SearchCharacteristics = searchCharacteristics,
                EventStats = eventStats,
                ProfilePageStats = profilePageStats
            };
        }

        private SessionStatDto GetSessionStats(IEnumerable<DirectoryAnalyticMetric> metricData)
        {
            var sessionData = ApplySessionMultiplication(metricData);

            (var sessionNumberLabels, var sessionNumberCount) = GetSessionCount(sessionData);
            (var avgBounceRateLabels, var avgBounceRateCount) = GetWeightedAverage(sessionData, x => x.BounceRate);
            (var avgNewSessionLabels, var avgNewSessionCount) = GetWeightedAverage(sessionData, x => x.PercentNewSessions);
            (var avgSessionDurationLabels, var avgSessionDurationCount) = GetWeightedAverage(sessionData, x => x.AvgSessionDuration);
            // var avgSessionDurationCountLabel = avgSessionDurationCount.Select(x => $"{(int)(x / 60)}m {Convert.ToInt32(x % 60)}s").ToList(); //do in frontend

            return new()
            {
                SessionNumberLabels = sessionNumberLabels,
                SessionNumberCount = sessionNumberCount,
                AvgBounceRateLabels = avgBounceRateLabels,
                AvgBounceRateCount = avgBounceRateCount,
                AvgNewSessionLabels = avgNewSessionLabels,
                AvgNewSessionCount = avgNewSessionCount,
                AvgSessionDurationLabels = avgSessionDurationLabels,
                AvgSessionDurationCount = avgSessionDurationCount,

            };
        }

        private SessionStatDto GetSessionSearchStats(IEnumerable<DirectoryAnalyticMetric> metricData)
        {
            var searchData = metricData.Where(x => x.PagePath.Contains("/Search/"));
            var sessionData = ApplySessionMultiplication(searchData);

            var sessionSummary = GetSummary(sessionData, x => x.Sessions);
            (var sessionNumberLabels, var sessionNumberCount) = GetSessionCount(sessionData);
            (var avgBounceRateLabels, var avgBounceRateCount) = GetWeightedAverage(sessionData, x => x.BounceRate);
            (var avgNewSessionLabels, var avgNewSessionCount) = GetWeightedAverage(sessionData, x => x.PercentNewSessions);
            (var avgSessionDurationLabels, var avgSessionDurationCount) = GetWeightedAverage(sessionData, x => x.AvgSessionDuration);

            return new()
            {
                SessionNumberLabels = sessionNumberLabels,
                SessionNumberCount = sessionNumberCount,
                AvgBounceRateLabels = avgBounceRateLabels,
                AvgBounceRateCount = avgBounceRateCount,
                AvgNewSessionLabels = avgNewSessionLabels,
                AvgNewSessionCount = avgNewSessionCount,
                AvgSessionDurationLabels = avgSessionDurationLabels,
                AvgSessionDurationCount = avgSessionDurationCount,
            };
        }

        private SearchCharacteristicDto GetSearchCharacteristics(IEnumerable<OrganisationAnalytic> biobankData)
        {
            var searchData = biobankData.Where(x => x.PagePath.Contains("/Search/"));

            (var searchTypeLabels, var searchTypeCount) =
                _transform.GetSearchBreakdown(searchData, _transform.GetSearchType);
            (var searchTermLabels, var searchTermCount) = _transform.GetSearchBreakdown(
                searchData.Where(x => _transform.GetSearchType(x.PagePath) == "Diagnosis"),
                _transform.GetSearchTerm);
            (var searchFilterLabels, var searchFilterCount) = _transform.GetSearchFilters(searchData);

            return new()
            {
                SearchTypeLabels = searchTypeLabels,
                SearchTypeCount = searchTypeCount,
                SearchTermLabels = searchTermLabels,
                SearchTermCount = searchTermCount,
                SearchFilterLabels = searchFilterLabels,
                SearchFilterCount = searchFilterCount
            };
        }

        private EventStatDto GetEventStats(IEnumerable<DirectoryAnalyticEvent> eventData)
        {
            var contactData = eventData.Where(x => x.EventAction == "Add Contact to List");
            var mailtoData = eventData.Where(x => x.EventAction == "Mailto clicked");
            (var contactNumberLabels, var contactNumberCount) = GetContactCount(contactData);
            (var filteredContactLabels, var filteredContactCount) = GetFilteredEventCount(contactData, _config.EventThreshold);
            (var filteredMailtoLabels, var filteredMailtoCount) = GetFilteredEventCount(mailtoData, _config.EventThreshold);

            return new()
            {
                ContactNumberLabels = contactNumberLabels,
                ContactNumberCount = contactNumberCount,
                FilteredContactLabels = filteredContactLabels,
                FilteredContactCount = filteredContactCount,
                FilteredMailToLabels = filteredMailtoLabels,
                FilteredMailToCount = filteredMailtoCount
            };
        }

        private ProfilePageStatDto GetProfilePageStats(IEnumerable<OrganisationAnalytic> biobankData)
        {
            var profileData = biobankData.Where(x => x.PagePath.Contains("/Profile/"));

            var profileSources = GetPageSources(profileData, _config.MetricThreshold);
            (var pageRoutes, var routeCount) = GetPageRoutes(profileData);


            return new()
            {
                ProfileSources = profileSources,
                PageRouteLabels = pageRoutes,
                RouteCount = routeCount
            };
        }

        private IList<SourceCountDto> GetPageSources(IEnumerable<OrganisationAnalytic> biobankData, int numOfTopSources)
        {
            var totalCount = biobankData.Count();
            var sources = biobankData.GroupBy(
                x => x.Source,
                x => x.Counts,
                (key, group) => new SourceCountDto
                {
                    Source = key,
                    Count = group.Count(),
                    Percentage = group.Count() / Convert.ToDouble(totalCount) * 100
                }).OrderByDescending(x => x.Count).Take(numOfTopSources);

            sources = sources.Append(new SourceCountDto
            {
                Source = "Others",
                Count = totalCount - sources.Select(x => x.Count).Sum(),
                Percentage = 100.0 - sources.Select(x => x.Percentage).Sum()
            });

            return sources.ToList();
        }

        private (IList<string>, IList<int>) GetPageRoutes(IEnumerable<OrganisationAnalytic> biobankData)
        {
            var query = biobankData
                .GroupBy(
                    x => _transform.GetViewRoute(x.PreviousPagePath),
                    x => x.Counts,
                    (key, group) => (key, count: group.Sum()))
                .OrderByDescending(x => x.count)
                .ToList();

            return (query.ConvertAll(x => x.key), query.ConvertAll(x => x.count));
        }

        private IEnumerable<DirectoryAnalyticMetric> ApplySessionMultiplication(IEnumerable<DirectoryAnalyticMetric> metricData)
            => metricData.Select(x => new DirectoryAnalyticMetric
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

        private (IList<string>, IList<int>) GetSessionCount(IEnumerable<DirectoryAnalyticMetric> sessionData)
        {
            var sessionSummary = GetSummary(sessionData, x => x.Sessions);
            var quarterLabels = sessionSummary.Select(x => x.Quarter).ToList();
            var quarterCounts = sessionSummary.Select(x => x.Count).ToList();
            return (quarterLabels, quarterCounts);
        }

        private (IList<string>, IList<double>) GetWeightedAverage(IEnumerable<DirectoryAnalyticMetric> sessionData, Func<DirectoryAnalyticMetric, int> elementSelector)
        {
            var sessionSummary = GetSummary(sessionData, x => x.Sessions);
            var elementSummary = GetSummary(sessionData, elementSelector);

            var quarterLabels = elementSummary.Select(x => x.Quarter).ToList();
            List<double> weightedAvg = new();

            foreach (var item in elementSummary)
            {
                var denom = Convert.ToDouble(sessionSummary
                    .Where(x => x.Quarter == item.Quarter)
                    .Select(x => x.Count)
                    .FirstOrDefault());
                if (denom > 0)
                    weightedAvg.Add(item.Count / denom);
                else
                    weightedAvg.Add(item.Count);
            }
            return (quarterLabels, weightedAvg);
        }

        private (IList<string>, IList<int>) GetContactCount(IEnumerable<DirectoryAnalyticEvent> eventData)
        {
            var eventSummary = eventData.GroupBy(
                x => x.Date.ToQuarterString(),
                x => x.Counts,
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

        private (IList<string>, IList<int>) GetFilteredEventCount(IEnumerable<DirectoryAnalyticEvent> eventData, int threshold)
        {
            var eventsPerCityPerDay = eventData.GroupBy(
                x => new { city = x.City, date = x.Date },
                x => x.Counts,
                (key, group) => new
                {
                    City = key.city,
                    Date = key.date,
                    Count = group.Sum(),
                });

            var summary = eventsPerCityPerDay
                .Where(x => x.Count <= threshold).GroupBy(
                x => x.Date.ToQuarterString(),
                x => x.Count,
                (key, group) => new QuarterlySummary
                {
                    Biobank = "Directory",
                    Quarter = key,
                    Count = group.Sum(),
                }).OrderBy(x => x.Quarter);

            return (summary.Select(x => x.Quarter).ToList(), summary.Select(x => x.Count).ToList());
        }

        private IEnumerable<QuarterlySummary> GetSummary(IEnumerable<DirectoryAnalyticMetric> metricData, Func<DirectoryAnalyticMetric, int> elementSelector)
            => metricData.GroupBy(
                x => x.Date.ToQuarterString(),
                elementSelector,
                (key, group) => new QuarterlySummary
                {
                    Biobank = "Directory",
                    Quarter = key,
                    Count = group.Sum(),
                }).OrderBy(x => x.Quarter);
    }
}
