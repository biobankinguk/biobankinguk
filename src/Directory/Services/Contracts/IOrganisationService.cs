using Biobanks.Entities.Data;
using Biobanks.Identity.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<IEnumerable<Organisation>> List(string wildcard = "", bool includeSuspended = true);
        Task<IEnumerable<Organisation>> ListForActivity(string name = "", bool includeSuspended = true);
        Task<IEnumerable<Organisation>> ListByUserId(string userId);
        Task<IEnumerable<Organisation>> ListByExternalIds(IList<string> biobankExternalIds);
        Task<IEnumerable<Organisation>> ListByAnonymousIdentifiers(IEnumerable<Guid> biobankAnonymousIdentifiers);

        Task<OrganisationType> GetBiobankOrganisationTypeAsync();

        Task<Organisation> Get(int biobankId);
        Task<Organisation> GetForIndexing(int biobankId);
        Task<Organisation> GetByName(string biobankName);


        Task<Organisation> GetByExternalId(string externalId);
        Task<Organisation> GetByExternalIdForSearch(string externalId);

        
        Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId);

        Task<bool> RegistrationRequestExists(string name);

        Task<bool> IsApiClient(int biobankId);

        Task<bool> IsSuspended(int biobankId);
        Task<bool> IsSuspendedByCapability(int capabilityId);
        Task<bool> IsSuspendedByCollection(int collectonId);
        Task<bool> IsSuspendedBySampleSet(int sampleSetId);

        Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedRegistrationRequests();
        Task<IEnumerable<OrganisationRegisterRequest>> ListOpenRegistrationRequests();

        Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalRegistrationRequests();

        Task<OrganisationRegisterRequest> GetRegistrationRequestByEmail(string email);


        // Write
        Task<Organisation> Create(Organisation biobank);
        Task<Organisation> Update(Organisation biobank);

        Task<OrganisationUser> AddUser(string userId, int organisationId);
        Task RemoveUser(string userId, int organisationId);

        Task<OrganisationRegisterRequest> AddRegistrationRequest(OrganisationRegisterRequest request);
        Task RemoveRegistrationRequest(OrganisationRegisterRequest request);

        Task<OrganisationRegisterRequest> UpdateRegistrationRequest(OrganisationRegisterRequest request);

        Task<OrganisationRegisterRequest> GetRegistrationRequest(int requestId);
        Task<OrganisationRegisterRequest> GetRegistrationRequestByName(string name);


        Task<Organisation> Suspend(int id);
        Task<Organisation> Unsuspend(int id);
        Task<bool> AddFunder(int funderId, int biobankId);
        Task RemoveFunder(int funderId, int biobankId);
        Task Delete(int id);


        Task<KeyValuePair<string, string>> GenerateNewApiClient(int biobankId, string clientName = null);
        Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int biobankId);

        Task<ApplicationUser> GetLastActiveUser(int organisationId);
    }
}
