using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Search;

public class DetailedCollectionSearchCollectionModel
{
    public int CollectionId { get; set; }

    public string OntologyTerm { get; set; }

    public string CollectionTitle { get; set; }

    public string Description { get; set; }

    public string StartYear { get; set; }

    public string AccessCondition { get; set; }
    public string CollectionType { get; set; }
    public string CollectionStatus { get; set; }
    public IEnumerable<string> ConsentRestrictions { get; set; }

    public IList<DetailedCollectionSearchSampleSetModel> SampleSets { get; set; }

    public IEnumerable<KeyValuePair<string, string>> AssociatedData { get; set; }
}
