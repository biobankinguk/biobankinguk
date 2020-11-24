using System.Collections.Generic;
using System.Linq;
using Directory.Search.Dto.Facets;
using Directory.Search.Dto.Documents;
using Directory.Search.Constants;
using Nest;
using Newtonsoft.Json;

namespace Directory.Search.Elastic
{
    public abstract class BaseElasticSearchProvider
    {
        #region Building the Search Queries

        protected static QueryContainer[] BuildSearchQueries<T>(string diagnosis, IEnumerable<SelectedFacet> selectedFacets)
            where T : BaseDocument
        {
            // Create new container for our search queries.
            var queries = new List<QueryContainer>
            {
                // Add the query for the root diagnosis search.
                string.IsNullOrWhiteSpace(diagnosis)
                    ? Query<T>.MatchAll()
                    : Query<T>.Term(p => p.Diagnosis, diagnosis.ToLower())
            };

            // If there are no selected facets then just return the original query.
            return selectedFacets == null ? queries.ToArray()
                // Otherwise add the selected facets to the queries.
                : AddFacetsToQueryList(queries, selectedFacets).ToArray();
        }

        protected static QueryContainer[] BuildSearchQueries<T>(string biobankExternalId, string diagnosis, IEnumerable<SelectedFacet> selectedFacets)
            where T : BaseDocument
        {
            // Build the list of queries using the base query builder.
            var queryList = BuildSearchQueries<T>(diagnosis, selectedFacets).ToList();

            // Add an extra query for the biobankId.
            queryList.Add(Query<T>.Term(p => p.BiobankExternalId, biobankExternalId));

            // Return the new list.
            return queryList.ToArray();
        }

        private static List<QueryContainer> AddFacetsToQueryList(List<QueryContainer> queries, IEnumerable<SelectedFacet> selectedFacets)
        {
            var facetDetails = FacetList.GetFacetDetails().ToList();

            foreach (var selectedFacet in selectedFacets)
            {
                var currentFacetDetail = facetDetails.FirstOrDefault(x => x.Name == selectedFacet.Name);

                if (currentFacetDetail == null)
                    continue;

                queries.Add(currentFacetDetail.NestedAggregation
                    ? Query<CollectionDocument>.Nested(n => // TODO why is this typed and is it a problem for capabilities?
                        n.Path(currentFacetDetail.NestedAggregationPath).Query(nq =>
                            nq.Term(
                                $"{currentFacetDetail.NestedAggregationPath}.{currentFacetDetail.NestedAggregationFieldName}",
                                selectedFacet.Value)))
                    : Query<CollectionDocument>.Term(selectedFacet.Name, selectedFacet.Value));
            }

            return queries;
        }

        #endregion

        protected static AggregationContainerDescriptor<T> AddFacetAggregations<T>(AggregationContainerDescriptor<T> aggregations) where T : class
        {
            var searchType = FacetList.GetSearchType(aggregations);

            var facetList = FacetList.GetFacetDetails(searchType);

            foreach (var agg in facetList)
            {
                if (agg.NestedAggregation)
                {
                    void AddNestedAgg(string name, string nestedName)
                    {
                        aggregations.Nested($"{name}Root", nr => nr.Path(agg.NestedAggregationPath)
                            .Aggregations(aggs => aggs.Terms(name, t => t.Size(SizeLimits.SizeMax)
                                .Field(string.Concat(agg.NestedAggregationPath, ".", nestedName))
                                .Aggregations(top => top.ReverseNested("toTop", rn => rn.Aggregations(rna => rna.Cardinality("biobankCount", c => c.Field("biobankId"))))))));
                    }

                    AddNestedAgg(agg.Name, agg.NestedAggregationFieldName);
                    if (agg.HasMetadata) AddNestedAgg(agg.MetadataName, agg.MetadataName);
                }
                else
                {
                    void AddAgg(string name)
                    {
                        aggregations.Terms(name, cp => cp.Field(name)
                            .Size(SizeLimits.SizeMax)
                            .Aggregations(sa => sa.Cardinality("biobankCount", bic => bic.Field("biobankId"))));
                    }

                    AddAgg(agg.Name);
                    if (agg.HasMetadata) AddAgg(agg.MetadataName);
                }
            }

            return aggregations;
        }

        #region Extract Facets

        protected static IEnumerable<Facet> ExtractFacets<T>(ISearchResponse<T> searchResult) where T : class
        {
            var searchType = FacetList.GetSearchType(searchResult);

            var facetList = FacetList.GetFacetDetails(searchType);

            return
                facetList.Select(
                    facet =>
                        facet.NestedAggregation
                            ? ExtractReverseNestedFacet(
                                facet.Name,
                                searchResult.Aggregations.Nested($"{facet.Name}Root").Terms(facet.Name),
                                facet.HasMetadata ? searchResult.Aggregations.Nested($"{facet.MetadataName}Root").Terms(facet.MetadataName) : null)
                            : ExtractFacet(
                                facet.Name,
                                searchResult.Aggregations.Terms(facet.Name),
                                facet.HasMetadata ? searchResult.Aggregations.Terms(facet.MetadataName) : null))
                    .ToList();
        }

        private static Facet ExtractFacet(string aggregateName,
            MultiBucketAggregate<KeyedBucket<string>> aggregate,
            MultiBucketAggregate<KeyedBucket<string>> metadataAggregate)
        {
            var facetDetail = FacetList.GetFacetDetail(aggregateName);
            var facetGroup = facetDetail?.GetGroup();

            //do json conversion once on all metadata items for this facet
            IEnumerable<dynamic> metadataItems = null;
            if (facetDetail?.HasMetadata ?? false)
                metadataItems = metadataAggregate?.Buckets.Select(x => JsonConvert.DeserializeObject<dynamic>(x.Key));

            return new Facet
            {
                GroupName = facetGroup?.Name ?? string.Empty,
                GroupCollapsedByDefault = facetGroup?.CollapsedByDefault ?? false,
                Name = aggregateName,
                Values = aggregate.Buckets.Select(
                    x =>
                    {
                        //Try and get metadata
                        var metadata = metadataItems?.FirstOrDefault(y => y.Name == x.Key);

                        return new FacetValue
                        {
                            Name = x.Key,
                            SortOrder = metadata?.SortOrder ?? string.Empty,
                            Value = x.Cardinality("biobankCount").Value
                        };

                    }).ToList(),
                GroupOrder = facetGroup?.SortOrder ?? 0,
                FacetOrder = facetDetail?.SortOrderWithinGroup ?? 0
            };
        }

        private static Facet ExtractReverseNestedFacet(string aggregateName,
            MultiBucketAggregate<KeyedBucket<string>> aggregate,
            MultiBucketAggregate<KeyedBucket<string>> metadataAggregate)
        {
            var facetDetail = FacetList.GetFacetDetail(aggregateName);
            var facetGroup = facetDetail?.GetGroup();

            //do json conversion once on all metadata items for this facet
            IEnumerable<dynamic> metadataItems = null;
            if (facetDetail?.HasMetadata ?? false)
                metadataItems = metadataAggregate?.Buckets.Select(x => JsonConvert.DeserializeObject<dynamic>(x.Key));

            return new Facet
            {
                GroupName = facetGroup?.Name ?? string.Empty,
                GroupCollapsedByDefault = facetGroup?.CollapsedByDefault ?? false,
                Name = aggregateName,
                Values = aggregate.Buckets.Select(
                    x =>
                    {
                        //Try and get metadata
                        var metadata = metadataItems?.FirstOrDefault(y => y.Name == x.Key);

                        return new FacetValue
                        {
                            Name = x.Key,
                            SortOrder = metadata?.SortOrder ?? string.Empty,
                            Value = x.ReverseNested("toTop").Cardinality("biobankCount").Value
                        };
                    }).ToList(),
                GroupOrder = facetGroup?.SortOrder ?? 0,
                FacetOrder = facetDetail?.SortOrderWithinGroup ?? 0
            };
        }

        #endregion
    }
}
