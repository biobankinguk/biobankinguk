using System.Collections.Generic;

namespace Biobanks.Analytics.Dto
{
    public class DirectoryReportDto
    {
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public int EventsPerCityThreshold { get; set; }

        public SessionStatDto SessionStats { get; set; }
        public SessionStatDto SessionSearchStats { get; set; }
        public SearchCharacteristicDto SearchCharacteristics { get; set; }
        public EventStatDto EventStats { get; set; }
        public ProfilePageStatDto ProfilePageStats { get; set; }
    }

    public class SessionStatDto
    {
        //use class for label count pair?
        public IList<string> SessionNumberLabels { get; set; }
        public IList<int> SessionNumberCount { get; set; }
        public IList<string> AvgBounceRateLabels { get; set; }
        public IList<double> AvgBounceRateCount { get; set; }
        public IList<string> AvgNewSessionLabels { get; set; }
        public IList<double> AvgNewSessionCount { get; set; }
        public IList<string> AvgSessionDurationLabels { get; set; }
        public IList<double> AvgSessionDurationCount { get; set; }
    }

    public class SearchCharacteristicDto
    {
        public IList<string> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<string> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<string> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }
    }

    public class EventStatDto
    {
        public IList<string> ContactNumberLabels { get; set; }
        public IList<int> ContactNumberCount { get; set; }
        public IList<string> FilteredContactLabels { get; set; }
        public IList<int> FilteredContactCount { get; set; }
        public IList<string> FilteredMailToLabels { get; set; }
        public IList<int> FilteredMailToCount { get; set; }
    }

    public class ProfilePageStatDto
    {
        public IList<SourceCountDto> ProfileSources { get; set; }
        public IList<string> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }
    }

    public class SourceCountDto
    {
        public string Source { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
