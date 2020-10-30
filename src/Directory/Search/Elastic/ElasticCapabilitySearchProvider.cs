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

    /// <inheritdoc cref="ICapabilitySearchProvider" />
    public class ElasticCapabilitySearchProvider : BaseElasticSearchProvider, ICapabilitySearchProvider
    {
        private readonly ElasticClient _client;
        private readonly (string collections, string capabilities) _indexNames;

        public ElasticCapabilitySearchProvider(
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

            _client = new ElasticClient(settings);
        }

        /// <inheritdoc />
        public async Task<List<int>> ListIds()
        {
            var results = _client.Search<CapabilityDocument>(s => s
                .MatchAll()
                .Source(false) //don't care about the document, just its index id in the metadata
                .Size(SizeLimits.SizeMax));

            return results.Hits.Select(x => int.Parse(x.Id)).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<string> ListOntologyTerms(string wildcard = "")
        {
            var capabilities = _client.Search<CapabilityDocument>(s => s
                .Query(q => q.Wildcard(p => p.Diagnosis, $"*{wildcard}*"))
                .Size(SizeLimits.SizeMax)
                .Aggregations(a => a
                    .Terms("diagnoses", t => t
                        .Field(p => p.Diagnosis))));

            return capabilities.Aggregations.Terms("diagnoses").Buckets.Select(x => x.Key);
        }

        /// <inheritdoc />
        public DirectorySearchResult Search(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
        {
            var searchResult = _client
                .Search<CapabilityDocument>(
                    s => s.Query(
                            q => q.Bool(
                                bq => bq
                                    .Must(BuildSearchQueries<CapabilityDocument>(
                                        ontologyTerm,
                                        selectedFacets))))
                        .Size(SizeLimits.SizeMax)
                        .Aggregations(a => BuildCapabilitySearchAggregations()));

            return new DirectorySearchResult
            {
                Biobanks = ExtractBiobankSearchSummaries(searchResult),
                Facets = ExtractFacets(searchResult)
            };
        }

        /// <inheritdoc />
        public BiobankCapabilityResult Search(string organisationExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets)
        {
            var searchResult = _client
                .Search<CapabilityDocument>(
                    s => s.Query(
                            q => q.Bool(
                                bq => bq
                                    .Must(BuildSearchQueries<CapabilityDocument>(
                                        organisationExternalId,
                                        ontologyTerm,
                                        selectedFacets))))
                        .Sort(x => x.Ascending(y => y.Id))
                        .Size(SizeLimits.SizeMax))
                .Documents;

            return ExtractBiobankCapabilityDetailSearchResults(searchResult.ToList());
        }

        /// <inheritdoc />
        public async Task<long> Count()
            => (await _client.CountAsync<CapabilityDocument>()).Count;

        private static AggregationContainerDescriptor<CapabilityDocument> BuildCapabilitySearchAggregations()
        {
            var aggregations = new AggregationContainerDescriptor<CapabilityDocument>();

            aggregations.Terms("biobanks",
                    bbn =>
                        bbn.Size(SizeLimits.SizeMax)
                        .Field(o => o.Biobank)
                        .Aggregations(
                            biobankId =>
                                biobankId.Terms("biobankExternalIds", bbid => bbid.Field(o => o.BiobankExternalId))));

            AddFacetAggregations(aggregations);

            return aggregations;
        }

        #region Extracting the search results

        private static IEnumerable<BiobankSummary> ExtractBiobankSearchSummaries(ISearchResponse<CapabilityDocument> searchResult)
        {
            return searchResult.Aggregations.Terms("biobanks").Buckets.Select(x => new BiobankSummary
            {
                Name = x.Key,
                ExternalId = x.Terms("biobankExternalIds").Buckets.First().Key
            }).ToList();
        }

        private static BiobankCapabilityResult ExtractBiobankCapabilityDetailSearchResults(IList<CapabilityDocument> searchResult)
        {
            var extract = new BiobankCapabilityResult();

            // Return an empty collection if there are no search results.
            if (searchResult == null || !searchResult.Any()) return extract;

            // Otherwise create the initial collection.
            var currentId = searchResult.First().Id;
            var currentCapability = BuildCapabilitySearchSummary(searchResult.First());

            // Add the biobank details.
            extract.BiobankId = searchResult.First().BiobankId;
            extract.BiobankExternalId = searchResult.First().BiobankExternalId;
            extract.BiobankName = searchResult.First().Biobank;

            // Loop around rest adding sample set and collections as the values change.
            foreach (var result in searchResult)
            {
                // When we hit a new collection...
                if (result.Id != currentId)
                {
                    // Add the current collection to the extract...
                    extract.Capabilities.Add(currentCapability);

                    // ...change the current collection Id...
                    currentId = result.Id;

                    // ...and create a new current collection.
                    currentCapability = BuildCapabilitySearchSummary(result);
                }

                // Add the currently result as a sample set summary.
                currentCapability.AssociatedData = result.AssociatedData.Select(x => new AssociatedDataSummary
                {
                    DataType = x.Text,
                    ProcurementTimeframe = x.Timeframe
                }).ToList();
            }

            // Add the current collection to the extract.
            extract.Capabilities.Add(currentCapability);

            return extract;
        }

        private static CapabilitySummary BuildCapabilitySearchSummary(CapabilityDocument document)
        {
            return new CapabilitySummary
            {
                Protocols = document.Protocols,
                AnnualDonorExpectation = document.AnnualDonorExpectation
            };
        }

        #endregion
    }
}
