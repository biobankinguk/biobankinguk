using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IBiobankReadService
    {
        Task<Collection> GetCollectionByIdAsync(int id);
        Task<bool> IsCollectionFromApi(int id);

        Task<Collection> GetCollectionByIdForIndexingAsync(int id);
        Task<Collection> GetCollectionWithSampleSetsByIdAsync(int id);
        Task<IEnumerable<int>> GetCollectionIdsByOntologyTermAsync(string ontologyTerm);
        Task<IEnumerable<Collection>> ListCollectionsAsync();
        Task<IEnumerable<Collection>> ListCollectionsAsync(int organisationId);
        Task<SampleSet> GetSampleSetByIdForIndexingAsync(int id);
        bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId);
        bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId);
        bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId);


        Task<bool> IsMaterialTypeAssigned(int id);

        [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding service."
        , false)]
        Task<int> GetExtractionProcedureMaterialDetailsCount(string id);
        Task<bool> IsExtractionProcedureInUse(string id);
        Task<int> GetMaterialTypeMaterialDetailCount(int id);

        Task<int> GetServiceOfferingOrganisationCount(int id);

        Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(IEnumerable<int> sampleSetIds);

        Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexDeletionAsync(IEnumerable<int> sampleSetIds);

        Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int modelBiobankId);


        Task<string> GetUnusedTokenByUser(string biobankUserId);

    }
}
