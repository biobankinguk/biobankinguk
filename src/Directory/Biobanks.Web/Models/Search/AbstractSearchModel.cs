using System.Collections.Generic;

namespace Biobanks.Web.Models.Search
{
    public abstract class AbstractSearchModel
    {
        public AbstractSearchModel()
        {
            SelectedFacets = new List<string>();
        }

        public string Diagnosis { get; set; }

        public IEnumerable<BiobankSearchSummaryModel> Biobanks { get; set; }

        public IEnumerable<SearchFacetModel> Facets { get; set; }

        public IList<string> SelectedFacets { get; set; }
    }
}