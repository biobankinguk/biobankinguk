namespace Biobanks.Submissions.Api.Models.Profile
{
    public class CollectionModel
    {
        public int Id { get; set; }

        public string OntologyTerm { get; set; }

        public int StartYear { get; set; }

        public string MaterialTypes { get; set; }

        public int SampleSetsCount { get; set; }
    }
}
