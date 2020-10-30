using Directory.Search.Dto.Facets;
using System.Collections.Generic;

namespace Directory.Search.Dto.Results
{
    public class Result
    {
        public IEnumerable<BiobankSummary> Biobanks { get; set; }

        public IEnumerable<Facet> Facets { get; set; }
    }
}
