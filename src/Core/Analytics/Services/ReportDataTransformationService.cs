using Biobanks.Analytics.Services.Contracts;
using Biobanks.Entities.Data.Analytics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Biobanks.Analytics.Services
{
    /// <inheritdoc />
    public class ReportDataTransformationService : IReportDataTransformationService
    {
        /// <inheritdoc />
        public (DateTimeOffset startDate, DateTimeOffset endDate) PeriodAsDateRange(int year, int endQuarter, int reportPeriod)
        {
            const int monthsPerQuarter = 3;
            var month = endQuarter * monthsPerQuarter;
            var lastDayofQuarter = DateTime.DaysInMonth(year, month);

            var endDate = new DateTimeOffset(year, month, lastDayofQuarter, 0, 0, 0, TimeSpan.Zero);
            //get start date by subtracting report period (specified in quarters) from end date
            var startDate = endDate.AddMonths(-1 * reportPeriod * monthsPerQuarter).AddDays(1);

            return (startDate, endDate);
        }

        public string GetViewRoute(string pagePath)
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

        public (IList<string>, IList<int>) GetSearchBreakdown(IEnumerable<OrganisationAnalytic> biobankData, Func<string, string> getSearchFunc)
        {
            var query = biobankData.GroupBy(
                    x => getSearchFunc(x.PagePath),
                    x => x.Counts,
                    (key, group) => (key, count: group.Sum()))
                .OrderByDescending(x => x.count)
                .ToList();

            return (query.ConvertAll(x => x.key), query.ConvertAll(x => x.count));
        }

        public string GetSearchType(string pagePath)
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

        public (IList<string>, IList<int>) GetSearchFilters(IEnumerable<OrganisationAnalytic> biobankData)
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

        private static string[] GetSearchFilters(string pagePath)
        {
            var pattern = @"selectedFacets=(\[(.*?)\])";
            return new Regex(pattern).Match(pagePath) switch
            {
                var m when m.Success
                    // TODO: are we sure the captured value has been URL decoded?!
                    // if not, it won't be valid JSON yet, as the `[` `]` will still be encoded
                    => JsonSerializer.Deserialize<string[]>(m.Groups[1].Captures[0].Value),
                _ => new[] { "" }
            };
        }

    }
}
