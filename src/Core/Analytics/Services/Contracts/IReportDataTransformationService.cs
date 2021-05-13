using Biobanks.Entities.Data.Analytics;

using System;
using System.Collections.Generic;

namespace Biobanks.Analytics.Services.Contracts
{

    /// <summary>
    /// This service has mathods for transforming report data needed by multiple reports
    /// </summary>
    public interface IReportDataTransformationService
    {
        string GetViewRoute(string pagePath);

        (IList<string>, IList<int>) GetSearchBreakdown(IEnumerable<OrganisationAnalytic> biobankData, Func<string, string> getSearchFunc);

        string GetSearchType(string pagePath);

        string GetSearchTerm(string pagePath);

        (IList<string>, IList<int>) GetSearchFilters(IEnumerable<OrganisationAnalytic> biobankData);
    }
}
