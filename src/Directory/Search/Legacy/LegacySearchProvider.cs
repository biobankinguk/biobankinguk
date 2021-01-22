using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Search.Constants;
using Biobanks.Search.Contracts;
using Biobanks.Search.Dto.Facets;
using Biobanks.Search.Dto.Results;

namespace Biobanks.Search.Legacy
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

        public Result CapabilitySearchBySnomedTerm(string snomedTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
            => _capabilities.Search(snomedTerm, selectedFacets, maxHits);

        public BiobankCapabilityResult CapabilitySearchBySnomedTermAndBiobank(string biobankExternalId, string snomedTerm, IEnumerable<SelectedFacet> selectedFacets)
            => _capabilities.Search(biobankExternalId, snomedTerm, selectedFacets);

        public Result CollectionSearchBySnomedTerm(string snomedTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits)
            => _collections.Search(snomedTerm, selectedFacets, maxHits);

        public BiobankCollectionResult CollectionSearchBySnomedTermAndBiobank(string biobankExternalId, string snomedTerm, IEnumerable<SelectedFacet> selectedFacets)
            => _collections.Search(biobankExternalId, snomedTerm, selectedFacets);

        public Task<long> CountCapabilitySearchDocuments()
            => _capabilities.Count();

        public Task<long> CountCollectionSearchDocuments()
            => _collections.Count();

        public Task<List<int>> GetAllCapabilityIds()
            => _capabilities.ListIds();

        public Task<List<int>> GetAllSampleSetIds()
            => _collections.ListIds();

        public IEnumerable<string> ListSnomedTerms(SearchDocumentType type, string wildcard = "")
            => type == SearchDocumentType.Capability
                ? _capabilities.ListOntologyTerms(wildcard)
                : _collections.ListOntologyTerms(wildcard);
    }
}
