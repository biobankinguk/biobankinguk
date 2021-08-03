using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class NetworkService : INetworkService
    {
        public Task<bool> AddBiobankToNetworkAsync(int biobankId, int networkId, string biobankExternalID, bool approve)
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkRegisterRequest> AddNetworkRegisterRequestAsync(NetworkRegisterRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkUser> AddUserToNetworkAsync(string userId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Network> CreateNetworkAsync(Network network)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteNetworkRegisterRequestAsync(NetworkRegisterRequest request)
        {
            throw new System.NotImplementedException();
        }

        public List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdForIndexingAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Network> GetNetworkByIdAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Network> GetNetworkByNameAsync(string networkName)
        {
            throw new System.NotImplementedException();
        }

        public List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkRegisterRequest> GetNetworkRegisterRequestAsync(int requestId)
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkRegisterRequest> GetNetworkRegisterRequestByUserEmailAsync(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Network>> GetNetworksByBiobankIdAsync(int organisationId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedNetworkRegisterRequestAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalNetworkRegisterRequestsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> ListNetworkAdminsAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Network>> ListNetworksAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<NetworkRegisterRequest>> ListOpenNetworkRegisterRequestsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> NetworkRegisterRequestExists(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveBiobankFromNetworkAsync(int biobankId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveUserFromNetworkAsync(string userId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Network> UpdateNetworkAsync(NetworkDTO networkDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<NetworkRegisterRequest> UpdateNetworkRegisterRequestAsync(NetworkRegisterRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<OrganisationNetwork> UpdateOrganisationNetworkAsync(OrganisationNetwork organisationNetwork)
        {
            throw new System.NotImplementedException();
        }
    }
}
