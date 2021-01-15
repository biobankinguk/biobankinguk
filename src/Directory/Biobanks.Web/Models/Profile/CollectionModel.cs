namespace Biobanks.Web.Models.Profile
{
    public class CollectionModel
    {
        public int Id { get; set; }

        public string SnomedTerm { get; set; }

        public int StartYear { get; set; }

        public string MaterialTypes { get; set; }

        public int SampleSetsCount { get; set; }
    }
}