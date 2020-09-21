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

        public ErrorStatusDto Error { get; set; }
    }
}
