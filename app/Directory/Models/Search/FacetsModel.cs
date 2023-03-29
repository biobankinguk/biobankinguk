using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Models.Search;

public class FacetsModel
{
    public string Action { get; set; }

    public string OntologyTerm { get; set; }

    public IEnumerable<SearchFacetModel> Facets { get; set; }

    public IList<string> SelectedFacets { get; set; }

    public IDictionary<string, List<string>> Countries { get; set; }

    public bool FacetSelected(string facetId) =>
        SelectedFacets != null &&
        SelectedFacets.Any(sf => sf == facetId);

    public List<string> GetCounties(string countryName)
    {             
        return Countries[countryName];
    }
    
    public string ShowCounties { get; set; }
    public string StorageTemperatureName { get; set; }
    public string MacroscopicAssessmentName { get; set; }
    public string DonorCount { get; set; }
}
