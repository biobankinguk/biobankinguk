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

        public ICollection<Country> Countries { get; set; }

        public bool FacetSelected(string facetId) =>
            SelectedFacets != null &&
            SelectedFacets.Any(sf => sf == facetId);

        public List<string> GetCounties(string countryName)
        {
            List<string> counties = new List<string>();

            foreach (var country in Countries)
            {
                if (country.Value == countryName)
                {
                    foreach (var county in country.Counties)
                    {
                        counties.Add(county.Value);
                    }
                }
            }           
            return counties;
        }
    }
}