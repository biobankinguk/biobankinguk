using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Directory.Search.Constants;
using Biobanks.Directory.Search.Contracts;
using Biobanks.Directory.Search.Dto.Facets;
using Biobanks.Directory.Search.Dto.Results;

namespace Biobanks.Directory.Search.Legacy
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

        public Result CapabilitySearchByOntologyTerm(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
            => _capabilities.Search(ontologyTerm, selectedFacets, maxHits);

        public BiobankCapabilityResult CapabilitySearchByOntologyTermAndBiobank(string biobankExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets)
            => _capabilities.Search(biobankExternalId, ontologyTerm, selectedFacets);

        public Result CollectionSearchByOntologyTerm(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
            => _collections.Search(ontologyTerm, selectedFacets, maxHits);

        public BiobankCollectionResult CollectionSearchByOntologyTermAndBiobank(string biobankExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets)
            => _collections.Search(biobankExternalId, ontologyTerm, selectedFacets);

        public Task<long> CountCapabilitySearchDocuments()
            => _capabilities.Count();

        public Task<long> CountCollectionSearchDocuments()
            => _collections.Count();

        public Task<List<int>> GetAllCapabilityIds()
            => _capabilities.ListIds();

        public Task<List<int>> GetAllSampleSetIds()
            => _collections.ListIds();

        public IEnumerable<OntologyTermsSummary> ListOntologyTerms(SearchDocumentType type, string wildcard = "")
            => type == SearchDocumentType.Capability
                ? _capabilities.ListOntologyTerms(wildcard)
                : _collections.ListOntologyTerms(wildcard);
    }
}
