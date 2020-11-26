using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Directory.Search.Contracts;
using Directory.Search.Dto.Facets;
using Directory.Search.Dto.Documents;
using Directory.Search.Dto.Results;
using Elasticsearch.Net;
using Nest;
using DirectorySearchResult = Directory.Search.Dto.Results.Result;
using Directory.Search.Constants;

namespace Directory.Search.Elastic
{
    // TODO major renaming work
    // diagnosis -> ontology term
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

            settings.DisableDirectStreaming(); // Debug

            _client = new ElasticClient(settings);
        }

        /// <inheritdoc />
        public async Task<List<int>> ListIds()
        {
            var results = _client.Search<CollectionDocument>(s => s
                .MatchAll()
                .Source(false) //don't care about the document, just its index id in the metadata
                .Size(SizeLimits.SizeMax));

            return results.Hits.Select(x => int.Parse(x.Id)).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<string> ListOntologyTerms(string wildcard = "")
        {
            var collections = _client.Search<CollectionDocument>(s => s
                .Query(q => q.Wildcard(p => p.Diagnosis, $"*{wildcard}*"))
                .Size(SizeLimits.SizeMax)
                .Aggregations(a => a
                    .Terms("diagnoses", t => t
                        .Field(p => p.Diagnosis))));

            return collections.Aggregations.Terms("diagnoses").Buckets.Select(x => x.Key);
        }

        /// <inheritdoc />
        public DirectorySearchResult Search(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
        {
            var searchResult =
                _client.Search<CollectionDocument>(
                    s => s.Query(q => q.Bool(
                            bq => bq.Must(BuildSearchQueries<CollectionDocument>(
                                ontologyTerm,
                                selectedFacets))))
                        .Size(SizeLimits.SizeMax)
                        .Aggregations(a => BuildCollectionSearchAggregations()));

            // Collect Biobanks Results
            var biobanks = ExtractBiobankSearchSummaries(searchResult);

            // Collect Search Facets
            var searchFacets = ExtractFacets(searchResult);
            var facets = AddConsentRestrictionsFacet(searchFacets, biobanks.Count()); // Consent Restriction Facet Opt-Out Edge Case

            return new DirectorySearchResult
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
            }).ToList();
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
                        PreservationType = mpd.PreservationType,
                        MacroscopicAssessment = mpd.MacroscopicAssessment,
                        PercentageOfSampleSet = mpd.PercentageOfSampleSet
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
                Diagnosis = document.Diagnosis,
                CollectionTitle = document.CollectionTitle,
                StartYear = document.StartYear,
                EndYear = document.EndYear,
                AccessCondition = document.AccessCondition,
                CollectionType = document.CollectionType,
                CollectionStatus = document.CollectionStatus,
                CollectionPoint = document.CollectionPoint,
                ConsentRestrictions = document.ConsentRestrictions.Select(x => x.Description)
            };
        }

        #endregion
    
        private IEnumerable<Facet> AddConsentRestrictionsFacet(IEnumerable<Facet> resultFacets, int resultsCount)
        {
            /*  Since the Consent Restrictions facet is Opt-Out (MUST_NOT) rather than Opt-In (MUST) facet, the
             *  facet details are not returned via the search results, as none of the search results will contain
             *  that facet. Therefore the facet details are manually gathered via a secondary request. */

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

            var consentResult = searchResult.Aggregations.Nested(facetDetail.Name).Terms(facetDetail.Name);
            var consentFacet = ExtractReverseNestedFacet(facetDetail.Name, consentResult, null);

            // Incomplete Consent Facet From Search Results
            var searchFacetValues = resultFacets.Where(x => x.Name == facetDetail.Name).First().Values;

            // Merge Consent Facet Values
            consentFacet.Values = consentFacet.Values.Select(fv =>
            {
                var searchFacetValue = searchFacetValues.Where(x => x.Name == fv.Name).FirstOrDefault();

                fv.Value = searchFacetValue?.Value ?? resultsCount;

                return fv;
            });

            // Combine All Facets
            var facets = resultFacets.Where(f => f.Name != "consentRestrictions").Append(consentFacet);

            return facets;
        }
    }
}
