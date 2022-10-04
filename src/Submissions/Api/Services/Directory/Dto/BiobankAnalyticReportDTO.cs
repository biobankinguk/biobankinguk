using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Services.Directory.Dto
{
    public class BiobankAnalyticReportDTO
    {
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public string Logo { get; set; }
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public ProfileStatusDTO BiobankStatus { get; set; }
        public ProfilePageViewsDTO ProfilePageViews { get; set; }
        public SearchActivityDTO SearchActivity { get; set; }
        public ContactRequestsDTO ContactRequests { get; set; }
    }

    public class ProfileStatusDTO
    {
        public int CollectionStatus { get; set; }
        public string CollectionStatusMessage { get; set; }
        public int CapabilityStatus { get; set; }
        public string CapabilityStatusMessage { get; set; }
    }

    public class ProfilePageViewsDTO
    {
        public IList<string> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDTO> ProfileQuarters { get; set; }
        public IList<int> ViewsPerQuarter { get; set; }
        public IList<double> ViewsAverages { get; set; }
        public IList<string> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }
    }

    public class SearchActivityDTO
    {
        public IList<string> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDTO> SearchQuarters { get; set; }
        public IList<int> SearchPerQuarter { get; set; }
        public IList<double> SearchAverages { get; set; }
        public IList<string> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<string> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<string> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }
    }

    public class ContactRequestsDTO
    {
        public IList<string> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDTO> ContactQuarters { get; set; }
        public IList<int> ContactsPerQuarter { get; set; }
        public IList<double> ContactAverages { get; set; }
    }

    public class QuarterlyCountsDTO
    {
        public string BiobankId { get; set; }
        public int Total { get; set; }
        public IList<int> QuarterCount { get; set; }
    }

    public class QuarterlySummary
    {
        public string Biobank { get; set; }
        public string Quarter { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }

    }
}
