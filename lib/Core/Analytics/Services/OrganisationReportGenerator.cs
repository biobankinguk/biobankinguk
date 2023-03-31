using Biobanks.Analytics.Dto;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Extensions;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Analytics;

namespace Biobanks.Analytics.Services
{
    public class OrganisationReportGenerator : IOrganisationReportGenerator
    {
        private readonly IGoogleAnalyticsReportingService _ga;
        private readonly IAnalyticsService _analytics;
        private readonly IReportDataTransformationService _transform;
        private readonly AnalyticsOptions _config;

        public OrganisationReportGenerator(
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
        public async Task<OrganisationReportDto> GetReport(string organisationId, int year, int quarter, int period)
        {
            var (startDate, endDate) = _transform.PeriodAsDateRange(year, quarter, period);
            var biobankData = await _analytics.GetOrganisationAnalytics(startDate, endDate);
            var eventData = await _analytics.GetAnalyticsEvents(startDate, endDate);

            //filter by host
            if (!string.IsNullOrEmpty(_config.FilterHostname))
            {
                biobankData = biobankData.Where(x => x.Hostname.Contains(_config.FilterHostname));
                eventData = eventData.Where(x => x.Hostname.Contains(_config.FilterHostname));
            }

            var pageViews = GetProfilePageViews(organisationId, biobankData);
            var searchActivity = GetSearchActivity(organisationId, biobankData);
            var contactRequests = GetContactRequests(organisationId, eventData);
          
            return new()
            {
                ExternalId = organisationId,
                Year = year,
                EndQuarter = quarter,
                ReportPeriod = period,
                NumOfTopBiobanks = _config.MetricThreshold,
                ProfilePageViews = pageViews,
                SearchActivity = searchActivity,
                ContactRequests = contactRequests,
            };
        }

        private ProfilePageViewsDto GetProfilePageViews(string biobankId, IEnumerable<OrganisationAnalytic> biobankData)
        {
            var profileData = biobankData.Where(x => x.PagePath.Contains("/Profile/"));
            var summary = GetSummary(profileData);
            var ranking = GetRankings(summary);
            (var quarterLabels, var topPageViews) = GetTopBiobanks(summary, ranking, biobankId, _config.MetricThreshold);
            (var viewsPerQuarter, var viewsAvgs) = GetQuarterlyAverages(summary, biobankId);
            (var pageRoutes, var routeCount) = GetPageRouteCounts(profileData.Where(x => x.OrganisationExternalId == biobankId));

            return new()
            {
                QuarterLabels = quarterLabels,
                ProfileQuarters = topPageViews,
                ViewsPerQuarter = viewsPerQuarter,
                ViewsAverages = viewsAvgs,
                PageRouteLabels = pageRoutes,
                RouteCount = routeCount
            };
        }

        private SearchActivityDto GetSearchActivity(string biobankId, IEnumerable<OrganisationAnalytic> biobankData)
        {
            var searchData = biobankData.Where(x => x.PagePath.Contains("/Search/") && x.OrganisationExternalId == biobankId);
            var bbSearchData = searchData.Where(x => x.OrganisationExternalId == biobankId);
            var summary = GetSummary(searchData);
            var ranking = GetRankings(summary);
            (var quarterLabels, var topSearches) = GetTopBiobanks(summary, ranking, biobankId, _config.MetricThreshold);
            (var searchPerQuarter, var searchAvgs) = GetQuarterlyAverages(summary, biobankId);
            (var searchTypeLabels, var searchTypeCount) =
                _transform.GetSearchBreakdown(bbSearchData, _transform.GetSearchType);
            (var searchTermLabels, var searchTermCount) = _transform.GetSearchBreakdown(
                    bbSearchData.Where(x => _transform.GetSearchType(x.PagePath) == "Diagnosis"),
                    _transform.GetSearchTerm);
            (var searchFilterLabels, var searchFilterCount) = _transform.GetSearchFilters(bbSearchData);

            return new()
            {
                QuarterLabels = quarterLabels,
                SearchQuarters = topSearches,
                SearchPerQuarter = searchPerQuarter,
                SearchAverages = searchAvgs,
                SearchTypeLabels = searchTypeLabels,
                SearchTypeCount = searchTypeCount,
                SearchTermLabels = searchTermLabels,
                SearchTermCount = searchTermCount,
                SearchFilterLabels = searchFilterLabels,
                SearchFilterCount = searchFilterCount
            };
        }

        private ContactRequestsDto GetContactRequests(string biobankId, IEnumerable<DirectoryAnalyticEvent> eventData)
        {
            var summary = GetSummary(eventData.Where(x => x.EventAction == "Add Contact to List"));
            var ranking = GetRankings(summary);
            (var quarterLabels, var topContactRequests) = GetTopBiobanks(summary, ranking, biobankId, _config.MetricThreshold);
            (var requestsPerQuarter, var requestsAvgs) = GetQuarterlyAverages(summary, biobankId);

            return new()
            {
                QuarterLabels = quarterLabels,
                ContactQuarters = topContactRequests,
                ContactsPerQuarter = requestsPerQuarter,
                ContactAverages = requestsAvgs,
            };
        }

        // would be nice to genericise GetSummary, but the property names differ between data types
        private IEnumerable<QuarterlySummary> GetSummary(IEnumerable<OrganisationAnalytic> biobankData)
            => biobankData.GroupBy(
                x => new { bb = x.OrganisationExternalId, q = x.Date.ToQuarterString() },
                x => x.Counts,
                (key, group) => new QuarterlySummary
                {
                    Biobank = key.bb,
                    Quarter = key.q,
                    Count = group.Sum(),
                });

        private IEnumerable<QuarterlySummary> GetSummary(IEnumerable<DirectoryAnalyticEvent> eventData)
            => eventData.GroupBy(
                x => new { bb = x.Biobank, q = x.Date.ToQuarterString() },
                x => x.Counts,
                (key, group) => new QuarterlySummary
                {
                    Biobank = key.bb,
                    Quarter = key.q,
                    Count = group.Sum(),
                });

        private IEnumerable<QuarterlySummary> GetRankings(IEnumerable<QuarterlySummary> summary)
            => summary.GroupBy(
                    x => x.Biobank,
                    x => x.Count,
                    (key, group) => new QuarterlySummary
                    {
                        Biobank = key,
                        Quarter = "Total",
                        Total = group.Sum()
                    }).OrderByDescending(y => y.Total);

        private (IList<string>, IList<QuarterlyCountsDto>) GetTopBiobanks(
            IEnumerable<QuarterlySummary> summary,
            IEnumerable<QuarterlySummary> ranking,
            string biobankId,
            int numOfTopBiobanks)
        {
            //Get Top Biobanks
            var getTopBiobanks = ranking.Take(numOfTopBiobanks);

            //if biobank isnt in top biobanks, append to list
            if (!getTopBiobanks.Any(x => x.Biobank == biobankId))
            {
                var bbRanking = ranking.Where(x => x.Biobank == biobankId);
                getTopBiobanks = bbRanking.Any()
                    ? getTopBiobanks.Append(bbRanking.First())
                    : getTopBiobanks.Append(new QuarterlySummary
                    {
                        Biobank = biobankId,
                        Quarter = "Total",
                        Total = 0,
                        Count = 0
                    });
            }

            // build table
            List<QuarterlyCountsDto> profileQuarters = new();
            var quarterLabels = summary.GroupBy(x => x.Quarter).OrderBy(x => x.Key).Select(x => x.Key).ToList();

            foreach (var bb in getTopBiobanks) //loop through top few biobanks
            {
                List<int> qcount = new();
                var quarterCount = summary.Where(x => x.Biobank == bb.Biobank)
                    .OrderBy(y => y.Quarter).Select(z => new { z.Count, z.Quarter });

                foreach (var ql in quarterLabels) //done using a loop to fill in '0's for quarters with no data
                    qcount.Add(quarterCount.Where(x => x.Quarter == ql).Select(x => x.Count).FirstOrDefault());

                profileQuarters.Add(new()
                {
                    BiobankId = bb.Biobank,
                    Total = bb.Total,
                    QuarterCount = qcount
                });
            }

            return (quarterLabels, profileQuarters);
        }

        private (IList<int>, IList<double>) GetQuarterlyAverages(IEnumerable<QuarterlySummary> summary, string biobankId)
        {
            List<double> bbAvg = new();
            List<int> bbVal = new();

            var bbCount = Convert.ToDouble(summary
                .GroupBy(x => x.Biobank)
                .Select(x => x.Key)
                .Count());

            var quarterLabels = summary
                .GroupBy(x => x.Quarter)
                .OrderBy(x => x.Key)
                .Select(x => x.Key)
                .ToList();

            var quarterCount = summary
                .Where(x => x.Biobank == biobankId)
                .OrderBy(y => y.Quarter)
                .Select(z => new { z.Quarter, z.Count });

            foreach (var ql in quarterLabels)
            {   //done using a loop to fill in '0's for quarters with no data
                //Sum/count used instead of Average() for same reason
                bbAvg.Add(summary
                    .Where(x => x.Quarter == ql)
                    .Select(x => x.Count)
                    .DefaultIfEmpty()
                    .Sum() / bbCount);

                bbVal.Add(quarterCount
                    .Where(x => x.Quarter == ql)
                    .Select(x => x.Count)
                    .FirstOrDefault());
            }

            return (bbVal, bbAvg);
        }

        private (IList<string>, IList<int>) GetPageRouteCounts(IEnumerable<OrganisationAnalytic> biobankData)
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
    }
}
