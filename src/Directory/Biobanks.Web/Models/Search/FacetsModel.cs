using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Web.Models.Search
{
    public class FacetsModel
    {
        public string Action { get; set; }

        public string OntologyTerm { get; set; }

        public IEnumerable<SearchFacetModel> Facets { get; set; }

        public IList<string> SelectedFacets { get; set; }

        public IDictionary<string, IList<string>> Countries { get; set; }

        public bool FacetSelected(string facetId) =>
            SelectedFacets != null &&
            SelectedFacets.Any(sf => sf == facetId);

        public IList<string> GetCounties(string countryName)
        {             
            return Countries[countryName];
        }
    }
}