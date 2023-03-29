using System.Collections.Generic;

namespace Biobanks.Directory.Models.Search;

public class BiobankSearchSummaryModel
{
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public double? CollectionCount { get; set; }
    public IEnumerable<string> SampleSetSummaries { get; set; }
}
