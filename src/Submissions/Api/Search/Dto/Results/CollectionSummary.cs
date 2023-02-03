using System.Collections.Generic;

namespace Biobanks.Search.Dto.Results
{
    public class CollectionSummary
    {
        public CollectionSummary()
        {
            SampleSets = new List<SampleSetSummary>();
        }

        public int CollectionId { get; set; }
        public string OntologyTerm { get; set; }
        public string CollectionTitle { get; set; }

        public string StartYear { get; set; }
        public string EndYear { get; set; }

        public string AccessCondition { get; set; }
        public string CollectionType { get; set; }
        public string CollectionStatus { get; set; }
        public IEnumerable<string> ConsentRestrictions { get; set; }

        public IList<SampleSetSummary> SampleSets { get; set; }
        public IList<AssociatedDataSummary> AssociatedData { get; set; }
    }
}