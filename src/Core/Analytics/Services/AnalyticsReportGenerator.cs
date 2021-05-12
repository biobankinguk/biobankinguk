using Biobanks.Analytics.Dto;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Entities.Data.Analytics;
using Biobanks.Extensions;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Biobanks.Analytics.Services
{
    public class AnalyticsReportGenerator : IAnalyticsReportGenerator
    {
        private readonly IGoogleAnalyticsReportingService _ga;
        private readonly IAnalyticsService _analytics;
        private readonly AnalyticsOptions _config;

        public AnalyticsReportGenerator(
            IGoogleAnalyticsReportingService ga,
            IAnalyticsService analytics,
            IOptions<AnalyticsOptions> options)
        {
            _ga = ga;
            _analytics = analytics;
            _config = options.Value;
        }

        #region Organisation Report
        // TODO: may be own service, not just region?

        /// <inheritdoc />
        public async Task<OrganisationReportDto> GetOrganisationReport(string organisationId, int year, int quarter, int period)
        {
            var (startDate, endDate) = _ga.GetDateRangeBounds(_ga.PeriodAsDateRange(year, quarter, period));
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
                ContactRequests = contactRequests
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

            return new ProfilePageViewsDto
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
            (var searchTypeLabels, var searchTypeCount) = GetSearchBreakdown(bbSearchData, GetSearchType);
            (var searchTermLabels, var searchTermCount) = GetSearchBreakdown(bbSearchData.Where(x =>
                                               GetSearchType(x.PagePath) == "Diagnosis"), GetSearchTerm);
            (var searchFilterLabels, var searchFilterCount) = GetSearchFilters(bbSearchData);

            return new SearchActivityDto
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

            return new ContactRequestsDto
            {
                QuarterLabels = quarterLabels,
                ContactQuarters = topContactRequests,
                ContactsPerQuarter = requestsPerQuarter,
                ContactAverages = requestsAvgs,
            };
        }

        #endregion

        #region Common Data Transformations

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
                    x => GetViewRoute(x.PreviousPagePath),
                    x => x.Counts,
                    (key, group) => (key, count: group.Sum()))
                .OrderByDescending(x => x.count)
                .ToList();

            return (query.ConvertAll(x => x.key), query.ConvertAll(x => x.count));
        }

        private string GetViewRoute(string pagePath)
            => pagePath switch
            {
                _ when pagePath.Contains("/Search/Collection")
                    => "Search Existing Samples Query",

                _ when pagePath.Contains("/Search/Collection")
                    => "Search Existing Samples Query",

                _ when pagePath.Contains("/Search/Collection")
                    => "Search Existing Samples Query",

                _ when pagePath.Contains("/Search/Collection")
                    => "Search Existing Samples Query",

                _ when pagePath.Contains("/Search/Collection")
                    => "Search Existing Samples Query",

                _ when (pagePath.Contains("/Biobank/") && !pagePath.Contains("/Profile/Biobank/"))
                        || pagePath.Contains("/ADAC/")
                        || pagePath.Contains("/Account/")
                        || pagePath.Contains("/Register/")
                    => "Account & Backend Management",

                _ => "Other"
            };

        private (IList<string>, IList<int>) GetSearchBreakdown(IEnumerable<OrganisationAnalytic> biobankData, Func<string, string> getSearchFunc)
        {
            var query = biobankData.GroupBy(
                    x => getSearchFunc(x.PagePath),
                    x => x.Counts,
                    (key, group) => (key, count: group.Sum()))
                .OrderByDescending(x => x.count)
                .ToList();

            return (query.ConvertAll(x => x.key), query.ConvertAll(x => x.count));
        }

        private string GetSearchType(string pagePath)
            => pagePath switch
            {
                _ when pagePath.Contains("ontologyTerm=")
                    => "Diagnosis", // TODO: update?
                _ when pagePath.Contains("selectedFacets=")
                    => "Filters", // of course, this isn't mutually exclusive with the above...
                _ => "No specific search"
            };

        public string GetSearchTerm(string pagePath)
            => pagePath switch
            {
                _ when pagePath.Contains("ontologyTerm=")
                    => new Regex("ontologyTerm=([^&]+)").Match(pagePath) switch
                    {
                        var m when m.Success => m.Groups[1].Captures[0].Value,
                        _ => ""
                    },
                _ when pagePath.Contains("selectedFacets=")
                    => "Filter",
                _ => "Other"
            };

        public string[] GetSearchFilters(string pagePath)
            => new Regex("selectedFacets=([^&]+)").Match(pagePath) switch
            {
                var m when m.Success
                    // TODO: are we sure the captured value has been URL decoded?!
                    // if not, it won't be valid JSON yet, as the `[` `]` will still be encoded
                    => JsonSerializer.Deserialize<string[]>(m.Groups[1].Captures[0].Value),
                _ => new[] { "" }
            };

        private (IList<string>, IList<int>) GetSearchFilters(IEnumerable<OrganisationAnalytic> biobankData)
        {
            List<string> filters = new();
            List<int> filterCount = new();

            foreach (var bb in biobankData)
            {
                foreach (var term in GetSearchFilters(bb.PagePath))
                {
                    var sterm = term.Split('_').LastOrDefault()?.TrimStart(' ').TrimEnd(' ');
                    if (string.IsNullOrEmpty(sterm))
                        continue;

                    if (!filters.Contains(sterm))
                    {
                        filters.Add(sterm);
                        filterCount.Add(1);
                    }
                    else
                    {
                        filterCount[filters.IndexOf(sterm)]++;
                    }
                }
            }
            return (filters, filterCount);
        }

        #endregion
    }
}