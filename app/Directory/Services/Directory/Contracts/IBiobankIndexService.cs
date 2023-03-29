using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IBiobankIndexService
    {
        Task BuildIndex();

        Task<string> GetClusterHealth();

        Task IndexSampleSet(int sampleSetId);
        Task IndexCapability(int capabilityId);
        Task UpdateSampleSetDetails(int sampleSetId);
        void DeleteSampleSet(int sampleSetId);
        void DeleteCapability(int capabilityId);

        void UpdateOrganisationDetails(Organisation organisation);
        void JoinOrLeaveNetwork(Organisation organisation);

        void UpdateNetwork(Network network);

        Task BulkIndexBiobank(Organisation organisation);
        void BulkDeleteBiobank(Organisation organisation);

        Task BulkIndexSampleSets(IList<int> sampleSetIds);
        Task BulkIndexCapabilities(IList<int> capabilityIds);
        void BulkDeleteSampleSets(IList<int> sampleSetIds);
        void BulkDeleteCapabilities(IList<int> capabilityIds);

        Task ClearIndex();
        
        /// <summary>
        /// Gets Capabilities that can be indexed.
        /// Filters out suspended organisations.
        /// </summary>
        /// <param name="capabilityIds">A list of capability Ids to check</param>
        /// <returns>A list of Capabilities.</returns>
        Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(IEnumerable<int> capabilityIds);
    }
}
