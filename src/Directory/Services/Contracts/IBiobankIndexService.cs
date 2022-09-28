using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public interface IBiobankIndexService
    {
        Task BuildIndex();

        Task<string> GetClusterHealth();

        Task IndexSampleSet(int sampleSetId);
        Task IndexCapability(int capabilityId);
        Task UpdateSampleSetDetails(int sampleSetId);
        Task UpdateCapabilityDetails(int capabilityId);
        Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm);
        void DeleteSampleSet(int sampleSetId);
        void DeleteCapability(int capabilityId);
        Task UpdateCollectionDetails(int collectionId);
        Task UpdateCollectionsOntologyOtherTerms(string ontologyTerm);
        
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
    }
}
