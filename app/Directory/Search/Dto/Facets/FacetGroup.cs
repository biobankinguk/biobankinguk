using Biobanks.Directory.Search.Constants;

namespace Biobanks.Directory.Search.Dto.Facets
{
    public class FacetGroup
    {
        public FacetGroupId Id { get; set; }
        public string Name { get; set; }
        public bool CollapsedByDefault { get; set; }
        public int SortOrder { get; set; }
    }
}
