using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Identity.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id, bool onlyDisplayable = false);

        Task<Blob> GetLogoBlobAsync(string logoName);
        Task<Collection> GetCollectionByIdAsync(int id);
        Task<bool> IsCollectionFromApi(int id);
        
        Task<Collection> GetCollectionByIdForIndexingAsync(int id);
        Task<Collection> GetCollectionWithSampleSetsByIdAsync(int id);
        Task<IEnumerable<int>> GetCollectionIdsByOntologyTermAsync(string ontologyTerm);
        Task<IEnumerable<Collection>> ListCollectionsAsync();
        Task<IEnumerable<Collection>> ListCollectionsAsync(int organisationId);
        Task<SampleSet> GetSampleSetByIdAsync(int id);
        Task<SampleSet> GetSampleSetByIdForIndexingAsync(int id);
        bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId);
        bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId);

        Task<DiagnosisCapability> GetCapabilityByIdAsync(int id);
        Task<DiagnosisCapability> GetCapabilityByIdForIndexingAsync(int id);
        Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm);
        Task<IEnumerable<DiagnosisCapability>> ListCapabilitiesAsync(int organisationId);
        bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId);

        IEnumerable<string> ExtractDistinctMaterialTypes(Collection collection);

        Task<bool> IsMaterialTypeAssigned(int id);

        Task<IEnumerable<OntologyTerm>> SearchOntologyTerms(string description = null, List<string> tags = null);
        Task<IEnumerable<OntologyTerm>> ListOntologyTerms(string description = null, List<string> tags = null, bool onlyDisplayable = false);
        Task<IEnumerable<OntologyTerm>> PaginateOntologyTerms(int start, int length, string description = null, List<string> tags = null);
        Task<OntologyTerm> GetOntologyTerm(string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false);
        Task<bool> ValidOntologyTerm(string id = null, string description = null, List<string> tags = null);
        Task<bool> IsOntologyTermInUse(string id);
        Task<int> CountOntologyTerms(string description = null, List<string> tags = null);
        Task<int> GetOntologyTermCollectionCapabilityCount(string id);

        Task<int> GetExtractionProcedureMaterialDetailsCount(string id);
        Task<bool> IsExtractionProcedureInUse(string id);

        Task<IEnumerable<SnomedTag>> ListSnomedTags();
        Task<SnomedTag> GetSnomedTagByDescription(string description);

        Task<int> GetMaterialTypeMaterialDetailCount(int id);

        Task<int> GetServiceOfferingOrganisationCount(int id);

        Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId);

        Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId);
        Task<IEnumerable<int>> GetAllSampleSetIdsAsync();
        Task<IEnumerable<int>> GetAllCapabilityIdsAsync();

        Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(IEnumerable<int> sampleSetIds);
        Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(IEnumerable<int> capabilityIds);

        /// <summary>
        /// Gets a count of all Sample Sets in the database.
        /// </summary>
        /// <returns>A count of all Sample Sets in the database.</returns>
        Task<int> GetSampleSetCountAsync();
        /// <summary>
        /// Gets a count of all Capabilities in the database.
        /// </summary>
        /// <returns>A count of all Capabilities in the database.</returns>
        Task<int> GetCapabilityCountAsync();

        Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexDeletionAsync(IEnumerable<int> sampleSetIds);
        Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexDeletionAsync(IEnumerable<int> capabilityIds);
        Task<int> GetIndexableSampleSetCountAsync();
        Task<int> GetIndexableCapabilityCountAsync();
        Task<int> GetSuspendedSampleSetCountAsync();
        Task<int> GetSuspendedCapabilityCountAsync();
        Task<Dictionary<int,string>> GetDescriptionsByCollectionIds(IEnumerable<int> collectionIds);

        Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId);

        Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int modelBiobankId);


        Task<string> GetUnusedTokenByUser(string biobankUserId);
        
    }
}
