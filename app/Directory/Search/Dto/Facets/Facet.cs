using System.Collections.Generic;

namespace Biobanks.Directory.Search.Dto.Facets
{
    public class Facet
    {
        public string GroupName { get; set; }
        public bool GroupCollapsedByDefault { get; set; }

        public string Name { get; set; }
        public IEnumerable<FacetValue> Values { get; set; }

        public int GroupOrder { get; set; }
        public int FacetOrder { get; set; }
    }
}
