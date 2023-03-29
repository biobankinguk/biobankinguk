using System.Collections.Generic;

namespace Biobanks.Search.Dto.Results
{
    public class BiobankCollectionResult
    {
        public BiobankCollectionResult()
        {
            Collections = new List<CollectionSummary>();
        }

        public int BiobankId { get; set; }
        public string BiobankExternalId { get; set; }
        public string BiobankName { get; set; }

        public IList<CollectionSummary> Collections { get; set; }
    }
}