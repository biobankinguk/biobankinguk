using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Search.Constants;
using Biobanks.Directory.Search.Contracts;
using Biobanks.Directory.Search.Dto.Documents;
using Biobanks.Directory.Search.Dto.Facets;
using Biobanks.Directory.Search.Dto.Results;
using Elasticsearch.Net;
using Nest;
using Result = Biobanks.Directory.Search.Dto.Results.Result;

namespace Biobanks.Directory.Search.Elastic
{
    // TODO major renaming work
    // biobank -> organisation

    /// <inheritdoc cref="ICollectionSearchProvider" />
    public class ElasticCollectionSearchProvider : BaseElasticSearchProvider, ICollectionSearchProvider
    {
        private readonly ElasticClient _client;
        private readonly (string collections, string capabilities) _indexNames;

        public ElasticCollectionSearchProvider(
            string elasticSearchUrl,
            (string collections, string capabilities) indexNames,
            string username,
            string password)
        {
            _indexNames = indexNames;

            var node = new Uri(elasticSearchUrl);
            var pool = new SingleNodeConnectionPool(node);

            var settings = new ConnectionSettings(pool)
                .DefaultMappingFor<CollectionDocument>(
                    m => m.IndexName(indexNames.collections))
                .DefaultMappingFor<CapabilityDocument>(
                    m => m.IndexName(indexNames.capabilities));

            // If there's a username and password for Basic Auth, use it
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                settings.BasicAuthentication(username, password);
            }
            
            settings.EnableApiVersioningHeader();

            _client = new ElasticClient(settings);
        }

        /// <inheritdoc />
        public Task<List<int>> ListIds()
        {
            var results = _client.Search<CollectionDocument>(s => s
                .MatchAll()
                .Source(false) //don't care about the document, just its index id in the metadata
                .Size(SizeLimits.SizeMax));

            return Task.FromResult(results.Hits.Select(x => int.Parse(x.Id)).ToList());
        }

        /// <inheritdoc />
        public IEnumerable<OntologyTermsSummary> ListOntologyTerms(string wildcard = "")
        {
            //Matches based on ontologyTerms and onTologyOtherTerms
            var collections = _client.Search<CollectionDocument>(s => s
                .Query(q => q.Wildcard(p => p.OntologyTerm, $"*{wildcard}*") || q.Nested(n => n
                .Path(p => p.OntologyOtherTerms).InnerHits(i => i.Explain()
                .Highlight(h => h.Fields(f => f.Field(o => o.OntologyOtherTerms.Select(on => on.Name)).PreTags("").PostTags(""))))
                .Query(nq => nq.Wildcard("ontologyOtherTerms.name", $"*{wildcard}*"))))
                .Size(SizeLimits.SizeMax)
                .Aggregations(a => a
                    .Terms("diagnoses", t => t
                        .Field(p => p.OntologyTerm))));

            return ExtractOntologyOtherTermsHits(collections);
        }

        /// <inheritdoc />
        public Result Search(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
        {
            var searchResult =
                _client.Search<CollectionDocument>(
                    s => s.Query(q => q.Bool(
                            bq => bq.Must(BuildSearchQueries<CollectionDocument>(
                                ontologyTerm,
                                selectedFacets))))
                        .Size(SizeLimits.SizeMax)
                        .Aggregations(a => BuildCollectionSearchAggregations()));

            if (!searchResult.IsValid)
                throw new ApplicationException(
                    $"Search Error: {searchResult.DebugInformation}",
                    searchResult.OriginalException);

            // Collect Biobanks Results
            var biobanks = ExtractBiobankSearchSummaries(searchResult);

            // Collect Search Facets
            var searchFacets = ExtractFacets(searchResult);
            var consentFacet = ExtractConsentRestrictionFacet(searchResult, searchFacets);

            var facets = searchFacets.Where(x => x.Name != "consentRestrictions").Append(consentFacet);

            return new Result
            {
                Biobanks = biobanks,
                Facets = facets
            };
        }

        /// <inheritdoc />
        public BiobankCollectionResult Search(string biobankExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets)
        {
            var searchResult =
                _client.Search<CollectionDocument>(
                        s => s.Query(
                                    q =>
                                        q.Bool(
                                            bq => bq.Must(BuildSearchQueries<CollectionDocument>(
                                                biobankExternalId,
                                                ontologyTerm,
                                                selectedFacets))))
                                .Sort(x => x.Ascending(y => y.CollectionId))
                                .Size(SizeLimits.SizeMax))
                    .Documents;

            // Extract the search results without Elastic's aggregations.
            return ExtractBiobankCollectionDetailSearchResults(searchResult.ToList());
        }

        /// <inheritdoc />
        public async Task<long> Count()
            => (await _client.CountAsync<CollectionDocument>()).Count;

        private static AggregationContainerDescriptor<CollectionDocument> BuildCollectionSearchAggregations()
        {
            var aggregations = new AggregationContainerDescriptor<CollectionDocument>();

            aggregations.Terms("biobanks",
                bbn =>
                    bbn.Size(SizeLimits.SizeMax)
                        .Field(o => o.Biobank)
                        .Order(x => x.Descending("collectionIds"))
                        .Aggregations(
                            biobankId =>
                                biobankId.Terms("biobankExternalIds", bbid => bbid.Field(o => o.BiobankExternalId))
                                    .Cardinality("collectionIds", bbid => bbid.Field(o => o.CollectionId))
                                    .Terms("sampleSetSummaries", sss => sss.Size(3).Field(o => o.SampleSetSummary))));

            AddFacetAggregations(aggregations);

            return aggregations;
        }

        #region Extracting the search results

        private static IEnumerable<BiobankSummary> ExtractBiobankSearchSummaries(ISearchResponse<CollectionDocument> searchResult)
        {
            return searchResult.Aggregations.Terms("biobanks").Buckets.Select(x => new BiobankSummary
            {
                Name = x.Key,
                ExternalId = x.Terms("biobankExternalIds").Buckets.First().Key,
                CollectionCount = x.Cardinality("collectionIds").Value,
                SampleSetSummaries = x.Terms("sampleSetSummaries").Buckets.Select(y => y.Key)
            });
        }

        private static BiobankCollectionResult ExtractBiobankCollectionDetailSearchResults(IList<CollectionDocument> searchResult)
        {
            var extract = new BiobankCollectionResult();

            // Return an empty collection if there are no search results.
            if (searchResult == null || !searchResult.Any()) return extract;

            // Otherwise create the initial collection.
            var currentCollectionId = searchResult.First().CollectionId;
            var currentCollection = BuildCollectionSearchSummary(searchResult.First());

            // Add the biobank details.
            extract.BiobankId = searchResult.First().BiobankId;
            extract.BiobankExternalId = searchResult.First().BiobankExternalId;
            extract.BiobankName = searchResult.First().Biobank;

            // Loop around rest adding sample set and collections as the values change.
            foreach (var result in searchResult)
            {
                // When we hit a new collection...
                if (result.CollectionId != currentCollectionId)
                {
                    // Add the current collection to the extract...
                    extract.Collections.Add(currentCollection);

                    // ...change the current collection Id...
                    currentCollectionId = result.CollectionId;

                    // ...and create a new current collection.
                    currentCollection = BuildCollectionSearchSummary(result);
                }

                // Add the currently result as a sample set summary.
                currentCollection.SampleSets.Add(new SampleSetSummary
                {
                    Sex = result.Sex,
                    AgeRange = result.AgeRange,
                    DonorCount = result.DonorCount,
                    MaterialPreservationDetails = result.MaterialPreservationDetails.Select(mpd => new MaterialPreservationDetailSummary
                    {
                        MaterialType = mpd.MaterialType,
                        StorageTemperature = mpd.StorageTemperature,
                        MacroscopicAssessment = mpd.MacroscopicAssessment,
                        PercentageOfSampleSet = mpd.PercentageOfSampleSet,
                        PreservationType = mpd.PreservationType,
                        ExtractionProcedure = mpd.ExtractionProcedure
                    })
                });
            }


            // Add the current collection to the extract.
            extract.Collections.Add(currentCollection);

            return extract;
        }

        private static CollectionSummary BuildCollectionSearchSummary(CollectionDocument document)
        {
            return new CollectionSummary
            {
                CollectionId = document.CollectionId,
                OntologyTerm = document.OntologyTerm,
                CollectionTitle = document.CollectionTitle,
                StartYear = document.StartYear,
                EndYear = document.EndYear,
                AccessCondition = document.AccessCondition,
                CollectionType = document.CollectionType,
                CollectionStatus = document.CollectionStatus,
                ConsentRestrictions = document.ConsentRestrictions.Select(x => x.Description),
                AssociatedData = document.AssociatedData.Select(x => new AssociatedDataSummary
                {
                    DataType = x.Text,
                    ProcurementTimeframe = x.Timeframe
                }).ToList()

        };
        }

        #endregion
        private Facet ExtractConsentRestrictionFacet(ISearchResponse<CollectionDocument> searchResponse, IEnumerable<Facet> searchFacets)
        {
            // Not All Facet Values Will Be Present As Facet Is Opt-Out, So We Gather Them Via Second Query
            var facetDetail = FacetList.GetFacetDetail("consentRestrictions");

            var searchResult = _client.Search<CollectionDocument>(s => s
                    .Aggregations(a => a
                        .Nested(facetDetail.Name, n => n
                            .Path(facetDetail.NestedAggregationPath)
                            .Aggregations(aa => aa
                                    .Terms(facetDetail.Name, t => t
                                    .Field(f => f.ConsentRestrictions.Suffix(facetDetail.NestedAggregationFieldName))
                                )
                             )
                        )
                    )
                );

            var facetResult = searchResult.Aggregations.Nested(facetDetail.Name).Terms(facetDetail.Name);
            var totalRestrictions = ExtractReverseNestedFacet(facetDetail.Name, facetResult, null); // All Values

            // Since Opt-Out We Need To Calculate The Cardinailty Manually 
            var collections = searchResponse.Documents;

            foreach (var facetValue in totalRestrictions.Values)
            {
                var isNoRestrictions = (facetValue.Name == "No restrictions"); // Edge Case Where Opt-In (Hence XOR)

                facetValue.Value = collections
                    .Where(x => isNoRestrictions ^ !x.ConsentRestrictions.Select(y => y.Description).Contains(facetValue.Name))
                    .GroupBy(x => x.BiobankId)
                    .Count();
            }

            return totalRestrictions;
        }
    }
}
