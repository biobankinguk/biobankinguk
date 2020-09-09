using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Services.Dto
{
    public class OrganisationAnalyticReportDto
    {
        public string ExternalId { get; set; }
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public ProfilePageViewsDto ProfilePageViews { get; set; }
        public SearchActivityDto SearchActivity { get; set; }
        public ContactRequestsDto ContactRequests { get; set; }
        public ErrorStatusDto Error { get; set; }
    }

    public partial class ProfilePageViewsDto
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDto> ProfileQuarters { get; set; }
        public IList<int> ViewsPerQuarter { get; set; }
        public IList<double> ViewsAverages { get; set; }
        public IList<String> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }
    }

    public partial class SearchActivityDto
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDto> SearchQuarters { get; set; }
        public IList<int> SearchPerQuarter { get; set; }
        public IList<double> SearchAverages { get; set; }
        public IList<String> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<String> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<String> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }
    }

    public partial class ContactRequestsDto
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCountsDto> ContactQuarters { get; set; }
        public IList<int> ContactsPerQuarter { get; set; }
        public IList<double> ContactAverages { get; set; }
    }

    public partial class ErrorStatusDto
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public partial class QuarterlyCountsDto
    {
        public string BiobankId { get; set; }
        public int Total { get; set; }
        public IList<int> QuarterCount { get; set; }
    }
}
