using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models;

public class BiobankAnalyticReport
{
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public string Logo { get; set; }
    public int Year { get; set; }
    public int EndQuarter { get; set; }
    public int ReportPeriod { get; set; }
    public int NumOfTopBiobanks { get; set; }
    public ProfileStatus BiobankStatus { get; set; }
    public ProfilePageViews ProfilePageViews { get; set; }
    public SearchActivity SearchActivity { get; set; }
    public ContactRequests ContactRequests { get; set; }

    public bool IsNullOrEmpty() =>
        ProfilePageViews.IsEmpty() &&
        SearchActivity.IsEmpty() &&
        ContactRequests.IsEmpty();
}

public class ProfileStatus
{
    public int CollectionStatus { get; set; }
    public string CollectionStatusMessage { get; set; }
    public int CapabilityStatus { get; set; }
    public string CapabilityStatusMessage { get; set; }
}

public class ProfilePageViews
{
    public IList<string> QuarterLabels { get; set; }
    public IList<QuarterlyCounts> ProfileQuarters { get; set; }
    public IList<int> ViewsPerQuarter { get; set; }
    public IList<double> ViewsAverages { get; set; }
    public IList<string> PageRouteLabels { get; set; }
    public IList<int> RouteCount { get; set; }

    public bool IsEmpty() => (
        ViewsPerQuarter.Count +
        RouteCount.Count +
        ViewsAverages.Count +
        ProfileQuarters.Count(x => x.QuarterCount.Count > 0))
        == 0;
}

public class SearchActivity
{
    public IList<string> QuarterLabels { get; set; }
    public IList<QuarterlyCounts> SearchQuarters { get; set; }
    public IList<int> SearchPerQuarter { get; set; }
    public IList<double> SearchAverages { get; set; }
    public IList<string> SearchTypeLabels { get; set; }
    public IList<int> SearchTypeCount { get; set; }
    public IList<string> SearchTermLabels { get; set; }
    public IList<int> SearchTermCount { get; set; }
    public IList<string> SearchFilterLabels { get; set; }
    public IList<int> SearchFilterCount { get; set; }

    public bool IsEmpty() => (
        SearchPerQuarter.Count +
        SearchAverages.Count +
        SearchTypeCount.Count +
        SearchTermCount.Count +
        SearchFilterCount.Count +
        SearchQuarters.Count(x => x.QuarterCount.Count > 0))
        == 0;
}

public class ContactRequests
{
    public IList<string> QuarterLabels { get; set; }
    public IList<QuarterlyCounts> ContactQuarters { get; set; }
    public IList<int> ContactsPerQuarter { get; set; }
    public IList<double> ContactAverages { get; set; }

    public bool IsEmpty() => (
        ContactsPerQuarter.Count +
        ContactAverages.Count +
        ContactQuarters.Count(x => x.QuarterCount.Count > 0))
        == 0;
}

public class QuarterlyCounts
{
    public string BiobankId { get; set; } //use external ID as this is serialized in HTML page 
    public int Total { get; set; }
    public IList<int> QuarterCount { get; set; }

    public bool IsEmpty() => QuarterCount.Count == 0;
}
