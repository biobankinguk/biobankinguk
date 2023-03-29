using System.Collections.Generic;

namespace Biobanks.Directory.Services.Directory.Dto
{
    public class DirectoryAnalyticReportDTO
    {
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }
        public int EventsPerCityThreshold { get; set; }

        public SessionStatDTO SessionStats { get; set; }
        public SessionStatDTO SessionSearchStats { get; set; }
        public SearchCharacteristicDTO SearchCharacteristics { get; set; }
        public EventStatDTO EventStats { get; set; }
        public ProfilePageStatDTO ProfilePageStats { get; set; }
    }

    public class SessionStatDTO
    {
        public List<string> SessionNumberLabels { get; set; }
        public List<int> SessionNumberCount { get; set; }
        public List<string> AvgBounceRateLabels { get; set; }
        public List<double> AvgBounceRateCount { get; set; }
        public List<string> AvgNewSessionLabels { get; set; }
        public List<double> AvgNewSessionCount { get; set; }
        public List<string> AvgSessionDurationLabels { get; set; }
        public IList<double> AvgSessionDurationCount { get; set; }
    }

    public class SearchCharacteristicDTO
    {
        public IList<string> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<string> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<string> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }
    }
    public class EventStatDTO
    {
        public IList<string> ContactNumberLabels { get; set; }
        public IList<int> ContactNumberCount { get; set; }
        public IList<string> FilteredContactLabels { get; set; }
        public IList<int> FilteredContactCount { get; set; }
        public IList<string> FilteredMailToLabels { get; set; }
        public IList<int> FilteredMailToCount { get; set; }
    }

    public class ProfilePageStatDTO
    {
        public IList<SourceCountDTO> ProfileSources { get; set; }
        public IList<string> PageRouteLabels { get; set; }
        public IList<int> RouteCount { get; set; }
    }

    public class SourceCountDTO
    {
        public string Source { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
