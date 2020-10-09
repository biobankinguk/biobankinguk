﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Data.Entities
{
    public class DirectoryAnalyticMetric
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string PagePath { get; set; }
        public string PagePathLevel1 { get; set; }
        public string Segment { get; set; }
        public string Source { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public int Sessions { get; set; }
        public int BounceRate { get; set; }
        public int PercentNewSessions { get; set; }
        public int AvgSessionDuration { get; set; }
    }
}
