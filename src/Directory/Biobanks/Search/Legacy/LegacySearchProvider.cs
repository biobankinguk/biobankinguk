using System.Collections.Generic;
using System.Threading.Tasks;
using Directory.Search.Contracts;
using Directory.Search.Dto.Facets;
using Directory.Search.Dto.Results;
using Directory.Search.Constants;

namespace Directory.Search.Legacy
{
    public class LegacySearchProvider : ISearchProvider
    {
        private readonly ICapabilitySearchProvider _capabilities;
        private readonly ICollectionSearchProvider _collections;

        public LegacySearchProvider(ICapabilitySearchProvider capabilities, ICollectionSearchProvider collections)
        {
            _capabilities = capabilities;
            _collections = collections;
        }

        public Result CapabilitySearchByDiagnosis(string diagnosis, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
            => _capabilities.Search(diagnosis, selectedFacets, maxHits);

        public BiobankCapabilityResult CapabilitySearchByDiagnosisAndBiobank(string biobankExternalId, string diagnosis, IEnumerable<SelectedFacet> selectedFacets)
            => _capabilities.Search(biobankExternalId, diagnosis, selectedFacets);

        public Result CollectionSearchByDiagnosis(string diagnosis, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
            => _collections.Search(diagnosis, selectedFacets, maxHits);

        public BiobankCollectionResult CollectionSearchByDiagnosisAndBiobank(string biobankExternalId, string diagnosis, IEnumerable<SelectedFacet> selectedFacets)
            => _collections.Search(biobankExternalId, diagnosis, selectedFacets);

        public Task<long> CountCapabilitySearchDocuments()
            => _capabilities.Count();

        public Task<long> CountCollectionSearchDocuments()
            => _collections.Count();

        public Task<List<int>> GetAllCapabilityIds()
            => _capabilities.ListIds();

        public Task<List<int>> GetAllSampleSetIds()
            => _collections.ListIds();

        public IEnumerable<string> ListDiagnoses(SearchDocumentType type, string wildcard = "")
            => type == SearchDocumentType.Capability
                ? _capabilities.ListOntologyTerms(wildcard)
                : _collections.ListOntologyTerms(wildcard);
    }
}
