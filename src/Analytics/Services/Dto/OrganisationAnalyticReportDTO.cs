using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Services.Dto
{
    public class OrganisationAnalyticReportDTO
    {
        public string ExternalId { get; set; }
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public ProfilePageViewsDTO ProfilePageViews { get; set; }
        public SearchActivityDTO SearchActivity { get; set; }
        public ContactRequestsDTO ContactRequests { get; set; }
        public ErrorStatusDTO Error { get; set; }
    }

    public partial class ProfileStatusDTO
    {
        public int CollectionStatus { get; set; }
        public string CollectionStatusMessage { get; set; }
        public int CapabilityStatus { get; set; }
        public string CapabilityStatusMessage { get; set; }
        public int HRA_HTAStatus { get; set; }
        public string HRA_HTAStatusMessage { get; set; }
    }

    public partial class ProfilePageViewsDTO
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDTO> ProfileQuarters { get; set; }
        public IList<int> ViewsPerQuarter { get; set; }
        public IList<double> ViewsAverages { get; set; }
        public IList<String> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }
    }

    public partial class SearchActivityDTO
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDTO> SearchQuarters { get; set; }
        public IList<int> SearchPerQuarter { get; set; }
        public IList<double> SearchAverages { get; set; }
        public IList<String> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<String> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<String> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }
    }

    public partial class ContactRequestsDTO
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDTO> ContactQuarters { get; set; }
        public IList<int> ContactsPerQuarter { get; set; }
        public IList<double> ContactAverages { get; set; }
    }

    public partial class ErrorStatusDTO
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public partial class QuarterlyCountsDTO
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
