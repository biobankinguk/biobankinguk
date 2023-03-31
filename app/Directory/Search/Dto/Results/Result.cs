using System.Collections.Generic;
using Biobanks.Directory.Search.Dto.Facets;

namespace Biobanks.Directory.Search.Dto.Results
{
    public class Result
    {
        public IEnumerable<BiobankSummary> Biobanks { get; set; }

        public IEnumerable<Facet> Facets { get; set; }
    }
}
