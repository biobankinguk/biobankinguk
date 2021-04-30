using System;

namespace Biobanks.Entities.Data.Analytics
{
    public class DirectoryAnalyticEvent
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
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
