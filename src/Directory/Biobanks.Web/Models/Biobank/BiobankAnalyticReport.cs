using Biobanks.Web.Models.Shared;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankAnalyticReport
    {
        public string Name { get; set; }
        public string ExternalId { get; set; } //maybe use external ID
        public string Logo { get; set; }
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public ProfileStatus BiobankStatus { get; set; }
        public ProfilePageViews ProfilePageViews { get; set; }
        public SearchActivity SearchActivity { get; set; }
        public ContactRequests ContactRequests { get; set; }
        public ErrorStatusModel Error { get; set; }

        public bool IsNullOrEmpty()
        {
            if ((ProfilePageViews == null || ProfilePageViews.IsEmpty()) &&
               (SearchActivity == null || SearchActivity.IsEmpty()) &&
               (ContactRequests == null || ContactRequests.IsEmpty()))
                return true;
            else
                return false;
        }
    }

    public partial class ProfileStatus
    {
        public int CollectionStatus { get; set; }
        public string CollectionStatusMessage { get; set; }
        public int CapabilityStatus { get; set; }
        public string CapabilityStatusMessage { get; set; }
        public int HRA_HTAStatus { get; set; }
        public string HRA_HTAStatusMessage { get; set; }
    }

    public partial class ProfilePageViews
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCounts> ProfileQuarters { get; set; }
        public IList<int> ViewsPerQuarter { get; set; }
        public IList<double> ViewsAverages { get; set; }
        public IList<String> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }

        public bool IsEmpty()
        {
            if ((this.ViewsPerQuarter?.Count() +
                this.RouteCount?.Count() +
                this.ViewsAverages?.Count() +
                this.ProfileQuarters?.Where(x => x.QuarterCount.Count > 0).Count()) == 0)
                return true;
            else
                return false;
        }
    }

    public partial class SearchActivity
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCounts> SearchQuarters { get; set; }
        public IList<int> SearchPerQuarter { get; set; }
        public IList<double> SearchAverages { get; set; }
        public IList<String> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<String> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<String> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }

        public bool IsEmpty()
        {
            if ((this.SearchPerQuarter?.Count() +
                this.SearchAverages?.Count() + 
                this.SearchTypeCount?.Count() +
                this.SearchTermCount?.Count() +
                this.SearchFilterCount?.Count() +
                this.SearchQuarters?.Where(x => x.QuarterCount.Count > 0).Count()) == 0)
                return true;
            else
                return false;
        }
    }

    public partial class ContactRequests
    {
        public IList<String> QuarterLabels { get; set; }
        public IList<QuarterlyCounts> ContactQuarters { get; set; }
        public IList<int> ContactsPerQuarter { get; set; }
        public IList<double> ContactAverages { get; set; }

        public bool IsEmpty()
        {
            if ((this.ContactsPerQuarter?.Count() +
                this.ContactAverages?.Count() +
                this.ContactQuarters?.Where(x => x.QuarterCount.Count > 0).Count()) == 0)
                return true;
            else
                return false;
        }
    }

    public partial class QuarterlyCounts
    {
        public string BiobankId { get; set; } //use external ID as this is serialized in HTML page 
        public int Total { get; set; }
        public IList<int> QuarterCount { get; set; }

        public bool IsEmpty()
        {
            if (this.QuarterCount?.Count() == 0)
                return true;
            else
                return false;
        }
    }
}