using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Entities
{
    public class DirectoryAnalyticEvent
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string EventCategory { get; set; }
        public string EventAction { get; set; }
        public string Biobank { get; set; } 
        public string Segment { get; set; }
        public string Source { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public int Counts { get; set; }
    }
}
