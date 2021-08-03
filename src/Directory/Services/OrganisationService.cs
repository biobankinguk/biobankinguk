using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class OrganisationService : IOrganisationService
    {
        public Task<Funder> AddFunderAsync(Funder funder)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddFunderToBiobankAsync(int funderId, int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<OrganisationRegisterRequest> AddRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OrganisationUser> AddUserToBiobankAsync(string userId, int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BiobankRegisterRequestExists(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> CreateBiobankAsync(OrganisationDTO biobank)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBiobankAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFunderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<string, string>> GenerateNewApiClientForBiobank(int biobankId, string clientName = null)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int biobankId)
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> GetBiobankByExternalIdAsync(string externalId)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> GetBiobankByExternalIdForSearchResultsAsync(string externalId)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> GetBiobankByIdAsync(int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> GetBiobankByIdForIndexingAsync(int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> GetBiobankByNameAsync(string biobankName)
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByUserEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BiobankActivityDTO>> GetBiobanksActivityAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> GetBiobanksByAnonymousIdentifiersAsync(IEnumerable<Guid> biobankAnonymousIdentifiers)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> GetBiobanksByExternalIdsAsync(IList<string> biobankExternalIds)
        {
            throw new NotImplementedException();
        }

        public List<Organisation> GetOrganisations()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsBiobankAnApiClient(int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsBiobankSuspendedAsync(int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCapabilityBiobankSuspendedAsync(int capabilityId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCollectionBiobankSuspendedAsync(int collectonId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSampleSetBiobankSuspendedAsync(int sampleSetId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedBiobankRegisterRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalBiobankRegisterRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegisterRequest>> ListOpenBiobankRegisterRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RegistrationReason>> ListRegistrationReasonsAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveFunderFromBiobankAsync(int funderId, int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromBiobankAsync(string userId, int biobankId)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> SuspendBiobankAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> UnsuspendBiobankAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Organisation> UpdateBiobankAsync(OrganisationDTO biobank)
        {
            throw new NotImplementedException();
        }

        public Task<Funder> UpdateFunderAsync(Funder funder)
        {
            throw new NotImplementedException();
        }

        public Task<OrganisationRegisterRequest> UpdateOrganisationRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganisationURLAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
