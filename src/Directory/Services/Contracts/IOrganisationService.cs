using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<Organisation> GetBiobankByIdAsync(int biobankId);
        Task<Organisation> GetBiobankByIdForIndexingAsync(int biobankId);
        Task<Organisation> GetBiobankByNameAsync(string biobankName);

        Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
        
        Task<Organisation> GetBiobankByExternalIdAsync(string externalId);
        Task<Organisation> GetBiobankByExternalIdForSearchResultsAsync(string externalId);
        Task<IEnumerable<Organisation>> GetBiobanksByExternalIdsAsync(IList<string> biobankExternalIds);

        Task<IEnumerable<Organisation>> GetBiobanksByAnonymousIdentifiersAsync(IEnumerable<Guid> biobankAnonymousIdentifiers);

        List<Organisation> GetOrganisations();

        List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId);


        Task<IEnumerable<RegistrationReason>> ListRegistrationReasonsAsync();
        Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId);

        Task<bool> BiobankRegisterRequestExists(string name);

        Task<bool> IsBiobankAnApiClient(int biobankId);

        Task<bool> IsBiobankSuspendedAsync(int biobankId);
        Task<bool> IsCapabilityBiobankSuspendedAsync(int capabilityId);
        Task<bool> IsCollectionBiobankSuspendedAsync(int collectonId);
        Task<bool> IsSampleSetBiobankSuspendedAsync(int sampleSetId);

        Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedBiobankRegisterRequestsAsync();
        Task<IEnumerable<OrganisationRegisterRequest>> ListOpenBiobankRegisterRequestsAsync();

        Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalBiobankRegisterRequestsAsync();

        Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByUserEmailAsync(string email);

        Task<IEnumerable<BiobankActivityDTO>> GetBiobanksActivityAsync();


        // Write
        Task<Organisation> CreateBiobankAsync(OrganisationDTO biobank);
        Task<Organisation> UpdateBiobankAsync(OrganisationDTO biobank);
        Task<OrganisationUser> AddUserToBiobankAsync(string userId, int biobankId);
        Task RemoveUserFromBiobankAsync(string userId, int biobankId);

        Task<OrganisationRegisterRequest> AddRegisterRequestAsync(OrganisationRegisterRequest request);
        Task DeleteRegisterRequestAsync(OrganisationRegisterRequest request);

        Task<OrganisationRegisterRequest> UpdateOrganisationRegisterRequestAsync(OrganisationRegisterRequest request);
        
        Task<Organisation> SuspendBiobankAsync(int id);
        Task<Organisation> UnsuspendBiobankAsync(int id);
        Task UpdateOrganisationURLAsync(int id);
        Task<bool> AddFunderToBiobankAsync(int funderId, int biobankId);
        Task RemoveFunderFromBiobankAsync(int funderId, int biobankId);
        Task DeleteBiobankAsync(int id);
        Task DeleteFunderByIdAsync(int id);
        Task<Funder> AddFunderAsync(Funder funder);
        Task<Funder> UpdateFunderAsync(Funder funder);

        Task<KeyValuePair<string, string>> GenerateNewApiClientForBiobank(int biobankId, string clientName = null);
        Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int biobankId);
    }
}
