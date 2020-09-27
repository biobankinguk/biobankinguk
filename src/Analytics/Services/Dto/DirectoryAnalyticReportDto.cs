using System;
using System.Collections.Generic;

namespace Analytics.Services.Dto
{
    public class DirectoryAnalyticReportDto
    {
        public int Year { get; set; }
        public int EndQuarter { get; set; }
        public int ReportPeriod { get; set; }
        public int NumOfTopBiobanks { get; set; }

        public SessionStatDto SessionStats { get; set; }
        public SessionStatDto SessionSearchStats { get; set; }
        public SearchCharacteristicDto SearchCharacteristics { get; set; }

        public ErrorStatusDto Error { get; set; }
    }

    public partial class SessionStatDto
    {
        public List<string>  SessionNumberLabels { get; set; }
        public List<int>    SessionNumberCount { get; set; }
        public List<string> AvgBounceRateLabels { get; set; }
        public List<double> AvgBounceRateCount { get; set; }
        public List<string> AvgNewSessionLabels { get; set; }
        public List<double> AvgNewSessionCount { get; set; }
        public List<string> AvgSessionDurationLabels { get; set; }
        public List<string> AvgSessionDurationCount { get; set; }
    }

    public partial class SearchCharacteristicDto
    {
        public IList<String> SearchTypeLabels { get; set; }
        public IList<int> SearchTypeCount { get; set; }
        public IList<String> SearchTermLabels { get; set; }
        public IList<int> SearchTermCount { get; set; }
        public IList<String> SearchFilterLabels { get; set; }
        public IList<int> SearchFilterCount { get; set; }
    }
}
