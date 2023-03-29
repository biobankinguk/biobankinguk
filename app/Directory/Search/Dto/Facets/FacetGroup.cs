using Biobanks.Search.Constants;

namespace Biobanks.Search.Dto.Facets
{
    public class FacetGroup
    {
        public FacetGroupId Id { get; set; }
        public string Name { get; set; }
        public bool CollapsedByDefault { get; set; }
        public int SortOrder { get; set; }
    }
}
