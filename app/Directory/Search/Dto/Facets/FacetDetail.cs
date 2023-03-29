using System.Collections.Generic;
using Biobanks.Directory.Search.Constants;

namespace Biobanks.Directory.Search.Dto.Facets
{
    public class FacetDetail
    {
        private readonly FacetGroupId _facetGroupId;

        public FacetDetail() { }

        public FacetDetail(FacetGroupId facetGroupId)
        {
            _facetGroupId = facetGroupId;
        }

        public FacetGroup GetGroup() => FacetList.GetFacetGroup(_facetGroupId);

        public string Label { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public bool HasMetadata { get; set; }
        public string MetadataName => HasMetadata ? $"{Name}Metadata" : null;

        public bool NestedAggregation { get; set; }
        public string NestedAggregationPath { get; set; }
        public string NestedAggregationFieldName { get; set; }

        public IEnumerable<SearchType> SearchTypes { get; set; }
        public int SortOrderWithinGroup { get; set; }
    }
}
