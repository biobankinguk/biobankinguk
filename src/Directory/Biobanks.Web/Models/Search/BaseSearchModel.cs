﻿using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Web.Models.Search
{
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

        public IList<IList<string>> Counties { get; set; }
    }
}