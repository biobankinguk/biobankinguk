using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IBiobankIndexService
    {
        Task BuildIndex();

        Task<string> GetClusterHealth();

        Task IndexSampleSet(int sampleSetId);
        Task IndexCapability(int capabilityId);
        Task UpdateSampleSetDetails(int sampleSetId);
        Task UpdateCapabilityDetails(int capabilityId);
        void DeleteSampleSet(int sampleSetId);
        void DeleteCapability(int capabilityId);
        Task UpdateCollectionDetails(int collectionId);
        Task UpdateBiobankDetails(int biobankId);
        Task UpdateNetwork(int networkId);
        Task JoinOrLeaveNetwork(int biobankId);

        Task BulkIndexBiobank(int biobankId);
        Task BulkIndexSampleSets(IList<int> sampleSetIds);
        Task BulkIndexCapabilities(IList<int> capabilityIds);
        Task BulkDeleteBiobank(int biobankId);
        void BulkDeleteSampleSets(IList<int> sampleSetIds);
        void BulkDeleteCapabilities(IList<int> capabilityIds);

        Task ClearIndex();
    }
}
