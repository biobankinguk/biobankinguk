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
        Result CollectionSearchByDiagnosis(string diagnosis, IEnumerable<SelectedFacet> selectedFacets, int maxHits);
        BiobankCollectionResult CollectionSearchByDiagnosisAndBiobank(string biobankExternalId, string diagnosis, IEnumerable<SelectedFacet> selectedFacets);
        Result CapabilitySearchByDiagnosis(string diagnosis, IEnumerable<SelectedFacet> selectedFacets, int maxHits);
        BiobankCapabilityResult CapabilitySearchByDiagnosisAndBiobank(string biobankExternalId, string diagnosis, IEnumerable<SelectedFacet> selectedFacets);
        Task<long> CountCollectionSearchDocuments();
        Task<long> CountCapabilitySearchDocuments();

        //List Indexed Id's for all documents of a type
        Task<List<int>> GetAllSampleSetIds();
        Task<List<int>> GetAllCapabilityIds();

        //List Diagnoses in the index, for a given document type
        IEnumerable<string> ListDiagnoses(SearchDocumentType type, string wildcard = "");
    }
}
