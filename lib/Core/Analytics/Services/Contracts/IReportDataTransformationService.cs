using System;
using System.Collections.Generic;
using Biobanks.Data.Entities.Analytics;

namespace Biobanks.Analytics.Services.Contracts
{

    /// <summary>
    /// This service has mathods for transforming report data needed by multiple reports
    /// </summary>
    public interface IReportDataTransformationService
    {
        /// <summary>
        /// Get DateTimeOffset bounds for a range of dates from
        /// parameters describing a reporting period
        /// </summary>
        /// <param name="year"></param>
        /// <param name="endQuarter"></param>
        /// <param name="reportPeriod"></param>
        (DateTimeOffset startDate, DateTimeOffset endDate) PeriodAsDateRange(int year, int endQuarter, int reportPeriod);

        string GetViewRoute(string pagePath);

        (IList<string>, IList<int>) GetSearchBreakdown(IEnumerable<OrganisationAnalytic> biobankData, Func<string, string> getSearchFunc);

        string GetSearchType(string pagePath);

        string GetSearchTerm(string pagePath);

        (IList<string>, IList<int>) GetSearchFilters(IEnumerable<OrganisationAnalytic> biobankData);
    }
}
