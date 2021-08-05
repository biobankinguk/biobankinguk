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
        Task<IEnumerable<Organisation>> List(string wildcard = "", bool includeSuspended = true);

        Task<OrganisationType> GetBiobankOrganisationTypeAsync();

        Task<Organisation> Get(int biobankId);
        Task<Organisation> GetForIndexing(int biobankId);
        Task<Organisation> GetByName(string biobankName);


        Task<Organisation> GetByExternalId(string externalId);
        Task<Organisation> GetByExternalIdForSearch(string externalId);
        Task<IEnumerable<Organisation>> ListByExternalIds(IList<string> biobankExternalIds);

        Task<IEnumerable<Organisation>> ListByAnonymousIdentifiers(IEnumerable<Guid> biobankAnonymousIdentifiers);


        List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId);


        
        Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId);

        Task<bool> RegistrationRequestExists(string name);

        Task<bool> IsApiClient(int biobankId);

        Task<bool> IsSuspended(int biobankId);
        Task<bool> IsCapabilityBiobankSuspendedAsync(int capabilityId);
        Task<bool> IsCollectionBiobankSuspendedAsync(int collectonId);
        Task<bool> IsSampleSetBiobankSuspendedAsync(int sampleSetId);

        Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedBiobankRegisterRequestsAsync();
        Task<IEnumerable<OrganisationRegisterRequest>> ListOpenBiobankRegisterRequestsAsync();

        Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalBiobankRegisterRequestsAsync();

        Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByUserEmailAsync(string email);

        Task<IEnumerable<BiobankActivityDTO>> GetBiobanksActivityAsync();


        // Write
        Task<Organisation> Create(Organisation biobank);
        Task<Organisation> Update(Organisation biobank);
        Task<OrganisationUser> AddUser(string userId, int biobankId);
        Task RemoveUser(string userId, int biobankId);

        Task<OrganisationRegisterRequest> AddRegistrationRequest(OrganisationRegisterRequest request);
        Task RemoveRegistrationRequest(OrganisationRegisterRequest request);

        Task<OrganisationRegisterRequest> UpdateOrganisationRegisterRequestAsync(OrganisationRegisterRequest request);
        
        Task<Organisation> Suspend(int id);
        Task<Organisation> Unsuspend(int id);
        Task<bool> AddFunder(int funderId, int biobankId);
        Task RemoveFunder(int funderId, int biobankId);
        Task Delete(int id);


        Task<KeyValuePair<string, string>> GenerateNewApiClient(int biobankId, string clientName = null);
        Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int biobankId);
    }
}
