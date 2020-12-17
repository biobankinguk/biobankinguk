using System;

namespace Directory.Entity.Data
{
    public class OrganisationAnalytic
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string PagePath { get; set; }
        public string PreviousPagePath { get; set; }
        public string Segment { get; set; }
        public string Source { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public int Counts { get; set; }
        public string OrganisationExternalId { get; set; }
    }
}
