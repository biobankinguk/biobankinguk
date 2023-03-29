using System.Collections.Generic;

namespace Biobanks.Directory.Models.Search;

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
    
    public string ShowCounties { get; set; }
    public string StorageTemperatureName { get; set; }
    public string MacroscopicAssessmentName { get; set; }
    public string DonorCount { get; set; }
}
