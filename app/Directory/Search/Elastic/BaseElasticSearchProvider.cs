using System.Collections.Generic;
using System.Linq;
using Biobanks.Directory.Search.Constants;
using Biobanks.Directory.Search.Dto.Documents;
using Biobanks.Directory.Search.Dto.Facets;
using Biobanks.Directory.Search.Dto.Results;
using Nest;
using Newtonsoft.Json;

namespace Biobanks.Directory.Search.Elastic
{
    public abstract class BaseElasticSearchProvider
    {
        #region Building the Search Queries

        protected static QueryContainer[] BuildSearchQueries<T>(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets)
            where T : BaseDocument
        {
            var ontologyTermQuery = string.IsNullOrWhiteSpace(ontologyTerm)
                    ? Query<T>.MatchAll()
                    : Query<T>.Term(p => p.OntologyTerm, ontologyTerm.ToLower());

            var facetQueries = selectedFacets != null
                    ? BuildFacetQueries(selectedFacets)
                    : Enumerable.Empty<QueryContainer>();

            return facetQueries.Prepend(ontologyTermQuery).ToArray();
        }

        protected static QueryContainer[] BuildSearchQueries<T>(string biobankExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets)
            where T : BaseDocument
        {
            // Build the list of queries using the base query builder.
            var queryList = BuildSearchQueries<T>(ontologyTerm, selectedFacets).ToList();

            // Add an extra query for the biobankId.
            queryList.Add(Query<T>.Term(p => p.BiobankExternalId, biobankExternalId));

            // Return the new list.
            return queryList.ToArray();
        }

        private static IEnumerable<QueryContainer> BuildFacetQueries(IEnumerable<SelectedFacet> selectedFacets)
        {
            var facetDetails = FacetList.GetFacetDetails();

            return selectedFacets
                .Where(f =>
                    facetDetails.Any(d => d.Name == f.Name)
                )
                .Select(f =>
                {
                    var facetDetail = facetDetails.First(d => d.Name == f.Name);

                    // Nested Facets
                    if (facetDetail.NestedAggregation)
                    {
                        var query = Query<CollectionDocument>.Nested(n =>
                                n.Path(facetDetail.NestedAggregationPath).Query(nq =>
                                    nq.Term(
                                        $"{facetDetail.NestedAggregationPath}.{facetDetail.NestedAggregationFieldName}",
                                        f.Value)
                                    )
                                );

                        // Consent Restrictions Work Opposite (NOT Facet) - Except for No Restrictions
                        if (f.Name == "consentRestrictions" && f.Value != "No restrictions")
                        {
                            query = !query;
                        }

                        return query;
                    }
                    // Non-Nested Facets
                    else
                    {
                        return Query<CollectionDocument>.Term(f.Name, f.Value);
                    }
                });
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

        protected static Facet ExtractFacet(string aggregateName,
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

        protected static Facet ExtractReverseNestedFacet(string aggregateName,
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
                            Value = x.ReverseNested("toTop")?.Cardinality("biobankCount").Value
                        };
                    }).ToList(),
                GroupOrder = facetGroup?.SortOrder ?? 0,
                FacetOrder = facetDetail?.SortOrderWithinGroup ?? 0
            };
        }

        #endregion
        protected static IEnumerable<OntologyTermsSummary> ExtractOntologyOtherTermsHits(ISearchResponse<BaseDocument> searchResponse)
        {
            var ontologyTerms = searchResponse.Aggregations
            .Terms("diagnoses")
            .Buckets
            .Select(x =>
                new OntologyTermsSummary
                {
                    OntologyTerm = x.Key,
                    MatchingOtherTerms = new List<string>()
                }).ToList();

            foreach (var ontologyTerm in ontologyTerms)
            {
                foreach (var hit in searchResponse.Hits.Where(x => x.Source.OntologyTerm.ToLower() == ontologyTerm.OntologyTerm))
                {
                    foreach (var terms in hit.InnerHits["ontologyOtherTerms"].Hits.Hits)
                    {
                        var hl = terms.Highlight["ontologyOtherTerms.name"].First();
                        if (!ontologyTerm.MatchingOtherTerms.Any(x => x == hl))
                            ontologyTerm.MatchingOtherTerms.Add(hl);
                    }
                }
            }

            return ontologyTerms;
        }
    }
}
