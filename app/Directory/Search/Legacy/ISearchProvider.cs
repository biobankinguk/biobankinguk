using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Directory.Search.Constants;
using Biobanks.Directory.Search.Dto.Facets;
using Biobanks.Directory.Search.Dto.Results;

namespace Biobanks.Directory.Search.Legacy
{
    // TODO: This is a legacy contract for bridging to the old index service code
    // It should be thrown away when the services are rewritten
    public interface ISearchProvider
    {
        Result CollectionSearchByOntologyTerm(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits);
        BiobankCollectionResult CollectionSearchByOntologyTermAndBiobank(string biobankExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets);
        
        Result CapabilitySearchByOntologyTerm(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits);
        BiobankCapabilityResult CapabilitySearchByOntologyTermAndBiobank(string biobankExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets);
        
        Task<long> CountCollectionSearchDocuments();
        Task<long> CountCapabilitySearchDocuments();

        //List Indexed Id's for all documents of a type
        Task<List<int>> GetAllSampleSetIds();
        Task<List<int>> GetAllCapabilityIds();

        //List OntologyTerms in the index, for a given document type
        IEnumerable<OntologyTermsSummary> ListOntologyTerms(SearchDocumentType type, string wildcard = "");
    }
}
