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

        public IList<IList<string>> Counties { get; set; }

        public bool FacetSelected(string facetId) =>
            SelectedFacets != null &&
            SelectedFacets.Any(sf => sf == facetId);
    }
}