using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Directory.Areas.Admin.Models.Analytics;

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
            SessionStats.IsEmpty() &&
            SessionSearchStats.IsEmpty() &&
            SearchCharacteristics.IsEmpty() &&
            EventStats.IsEmpty() &&
            ProfilePageStats.IsEmpty();
    }

    public class SessionStat
    {
        public List<string> SessionNumberLabels { get; set; }
        public List<int> SessionNumberCount { get; set; } = new();
        public List<string> AvgBounceRateLabels { get; set; }
        public List<double> AvgBounceRateCount { get; set; } = new();
        public List<string> AvgNewSessionLabels { get; set; }
        public List<double> AvgNewSessionCount { get; set; } = new();
        public List<string> AvgSessionDurationLabels { get; set; }
        public List<double> AvgSessionDurationCount { get; set; } = new();

        public bool IsEmpty() => (
            SessionNumberCount.Count +
            AvgBounceRateCount.Count +
            AvgNewSessionCount.Count +
            AvgSessionDurationCount.Count)
            == 0;
    }

    public class SearchCharacteristic
    {
        public List<string> SearchTypeLabels { get; set; }
        public List<int> SearchTypeCount { get; set; } = new();
        public List<string> SearchTermLabels { get; set; }
        public List<int> SearchTermCount { get; set; } = new();
        public List<string> SearchFilterLabels { get; set; }
        public List<int> SearchFilterCount { get; set; } = new();

        public bool IsEmpty() => (
            SearchTypeCount.Count +
            SearchTermCount.Count +
            SearchFilterCount.Count)
            == 0;
    }

    public class EventStat
    {
        public List<string> ContactNumberLabels { get; set; }
        public List<int> ContactNumberCount { get; set; } = new();
        public List<string> FilteredContactLabels { get; set; }
        public List<int> FilteredContactCount { get; set; } = new();
        public List<string> FilteredMailToLabels { get; set; }
        public List<int> FilteredMailToCount { get; set; } = new();

        public bool IsEmpty() => (
            ContactNumberCount.Count +
            FilteredContactCount.Count +
            FilteredMailToCount.Count)
            == 0;
    }

    public class ProfilePageStat
    {
      public List<SourceCount> ProfileSources { get; set; } = new();
      public List<string> PageRouteLabels { get; set; }
      public List<int> RouteCount { get; set; } = new();

        public bool IsEmpty() => (
                ProfileSources.Count == 0 ||
                (ProfileSources.Count == 1 &&
                ProfileSources.FirstOrDefault().Source == "Others" &&
                ProfileSources.FirstOrDefault().Percentage == 100 &&
                ProfileSources.FirstOrDefault().Count == 0)
            )
            && RouteCount.Count == 0;
    }

    public class SourceCount
    {
        public string Source { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
