using System.Collections.Generic;
using System.Linq;

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

        public bool IsNullOrEmpty() =>
            (SessionStats?.IsEmpty() != false) &&
            (SessionSearchStats?.IsEmpty() != false) &&
            (SearchCharacteristics?.IsEmpty() != false) &&
            (EventStats?.IsEmpty() != false) &&
            (ProfilePageStats?.IsEmpty() != false);
    }

    public class SessionStat
    {
        public List<string> SessionNumberLabels { get; set; }
        public List<int> SessionNumberCount { get; set; }
        public List<string> AvgBounceRateLabels { get; set; }
        public List<double> AvgBounceRateCount { get; set; }
        public List<string> AvgNewSessionLabels { get; set; }
        public List<double> AvgNewSessionCount { get; set; }
        public List<string> AvgSessionDurationLabels { get; set; }
        public IList<double> AvgSessionDurationCount { get; set; }

        public bool IsEmpty() => (
            SessionNumberCount?.Count() +
            AvgBounceRateCount?.Count() +
            AvgNewSessionCount?.Count() +
            AvgSessionDurationCount?.Count())
            == 0;
    }

    public class SearchCharacteristic
    {
        public IList<string> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<string> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<string> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }

        public bool IsEmpty() => (
            SearchTypeCount?.Count() +
            SearchTermCount?.Count() +
            SearchFilterCount?.Count())
            == 0;
    }

    public class EventStat
    {
        public IList<string> ContactNumberLabels { get; set; }
        public IList<int> ContactNumberCount { get; set; }
        public IList<string> FilteredContactLabels { get; set; }
        public IList<int> FilteredContactCount { get; set; }
        public IList<string> FilteredMailToLabels { get; set; }
        public IList<int> FilteredMailToCount { get; set; }

        public bool IsEmpty() => (
            ContactNumberCount?.Count() +
            FilteredContactCount?.Count() +
            FilteredMailToCount?.Count())
            == 0;
    }

    public class ProfilePageStat
    {
        public IList<SourceCount> ProfileSources { get; set; }
        public IList<string> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }

        public bool IsEmpty() => (
                ProfileSources?.Count() == 0 ||
                (ProfileSources?.Count() == 1 &&
                ProfileSources.FirstOrDefault().Source == "Others" &&
                ProfileSources.FirstOrDefault().Percentage == 100 &&
                ProfileSources.FirstOrDefault().Count == 0)
            )
            && RouteCount?.Count() == 0;
    }

    public class SourceCount
    {
        public string Source { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}