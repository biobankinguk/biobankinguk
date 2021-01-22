using System.Collections.Generic;
using Biobanks.Search.Dto.Facets;

namespace Biobanks.Search.Dto.Results
{
    public class Result
    {
        public IEnumerable<BiobankSummary> Biobanks { get; set; }

        public IEnumerable<Facet> Facets { get; set; }
    }
}
