using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Search;

public class BaseSearchModel
{
    public BaseSearchModel()
    {
        SelectedFacets = new List<string>();
    }

    public string OntologyTerm { get; set; }

    public IEnumerable<BiobankSearchSummaryModel> Biobanks { get; set; }

    public IEnumerable<SearchFacetModel> Facets { get; set; }

    public IList<string> SelectedFacets { get; set; }

    public IDictionary<string, List<string>> Countries { get; set; }
}
