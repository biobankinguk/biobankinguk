using Biobanks.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class DirectoryAnalyticReport
    {
        // consider making this a class as well in shared?
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public int EventsPerCityThreshold { get; set; }

        public SessionStat SessionStats { get; set; }
        public SessionStat SessionSearchStats { get; set; }
        public SearchCharacteristic SearchCharacteristics { get; set; }
        public EventStat EventStats { get; set; }
        public ProfilePageStat ProfilePageStats { get; set; }

        public ErrorStatusModel Error { get; set; }

        public bool IsNullOrEmpty()
        {
            if ((SessionStats == null || SessionStats.IsEmpty()) &&
               (SessionSearchStats == null || SessionSearchStats.IsEmpty()) &&
               (SearchCharacteristics == null || SearchCharacteristics.IsEmpty()) &&
               (EventStats == null || EventStats.IsEmpty()) &&
               (ProfilePageStats == null || ProfilePageStats.IsEmpty()) )
                return true;
            else
                return false;
        }
    }

    public partial class SessionStat
    {
        public List<string> SessionNumberLabels { get; set; }
        public List<int> SessionNumberCount { get; set; }
        public List<string> AvgBounceRateLabels { get; set; }
        public List<double> AvgBounceRateCount { get; set; }
        public List<string> AvgNewSessionLabels { get; set; }
        public List<double> AvgNewSessionCount { get; set; }
        public List<string> AvgSessionDurationLabels { get; set; }
        public IList<double> AvgSessionDurationCount { get; set; }

        public bool IsEmpty()
        {
            if ((this.SessionNumberCount?.Count() +
                this.AvgBounceRateCount?.Count() +
                this.AvgNewSessionCount?.Count() +
                this.AvgSessionDurationCount?.Count()) == 0)
                return true;
            else
                return false;
        }
    }

    public partial class SearchCharacteristic
    {
        public IList<String> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<String> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<String> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }

        public bool IsEmpty()
        {
            if ((this.SearchTypeCount?.Count() +
                this.SearchTermCount?.Count() +
                this.SearchFilterCount?.Count()) == 0)
                return true;
            else
                return false;
        }
    }

    public partial class EventStat
    {
        public IList<string> ContactNumberLabels { get; set; }
        public IList<int> ContactNumberCount { get; set; }
        public IList<string> FilteredContactLabels { get; set; }
        public IList<int> FilteredContactCount { get; set; }
        public IList<string> FilteredMailToLabels { get; set; }
        public IList<int> FilteredMailToCount { get; set; }

        public bool IsEmpty()
        {
            if ((this.ContactNumberCount?.Count() +
                this.FilteredContactCount?.Count() +
                this.FilteredMailToCount?.Count()) == 0)
                return true;
            else
                return false;
        }
    }

    public partial class ProfilePageStat
    {
        public IList<SourceCount> ProfileSources { get; set; }
        public IList<String> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }

        public bool IsEmpty()
        {
            if ((this.ProfileSources?.Count() == 0 ||
                (this.ProfileSources?.Count() == 1 &&
                this.ProfileSources.FirstOrDefault().Source == "Others" &&
                this.ProfileSources.FirstOrDefault().Percentage == 100 &&
                this.ProfileSources.FirstOrDefault().Count == 0)) &&
                this.RouteCount?.Count() == 0)
                return true;
            else
                return false;
        }
    }

    public class SourceCount
    {
        public string Source { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}