using System.Collections.Generic;

namespace Biobanks.Directory.Search.Dto.Results
{
    public class BiobankSummary
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public double? CollectionCount { get; set; }
        public IEnumerable<string> SampleSetSummaries { get; set; }
    }
}
