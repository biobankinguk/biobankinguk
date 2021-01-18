using System.Collections.Generic;
using System.Threading.Tasks;
using Directory.Search.Dto.Facets;
using Directory.Search.Dto.Results;
using Directory.Search.Constants;

namespace Directory.Search.Legacy
{
    // TODO: This is a legacy contract for bridging to the old index service code
    // It should be thrown away when the services are rewritten
    public interface ISearchProvider
    {
        Result CollectionSearchBySnomedTerm(string snomedTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits);
        BiobankCollectionResult CollectionSearchBySnomedTermAndBiobank(string biobankExternalId, string snomedTerm, IEnumerable<SelectedFacet> selectedFacets);
        
        Result CapabilitySearchBySnomedTerm(string snomedTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits);
        BiobankCapabilityResult CapabilitySearchBySnomedTermAndBiobank(string biobankExternalId, string snomedTerm, IEnumerable<SelectedFacet> selectedFacets);
        
        Task<long> CountCollectionSearchDocuments();
        Task<long> CountCapabilitySearchDocuments();

        //List Indexed Id's for all documents of a type
        Task<List<int>> GetAllSampleSetIds();
        Task<List<int>> GetAllCapabilityIds();

        //List SnomedTerms in the index, for a given document type
        IEnumerable<string> ListSnomedTerms(SearchDocumentType type, string wildcard = "");
    }
}
