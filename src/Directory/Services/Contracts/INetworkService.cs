using Biobanks.Entities.Data;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface INetworkService
    {

        Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdAsync(int networkId);
        Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdForIndexingAsync(int networkId);

        Task<Network> GetNetworkByIdAsync(int networkId);
        Task<Network> GetNetworkByNameAsync(string networkName);

        Task<IEnumerable<Network>> ListNetworksAsync();
        Task<IEnumerable<Network>> GetNetworksByBiobankIdAsync(int organisationId);

        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId);

        List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId);

        List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId);

        Task<bool> NetworkRegisterRequestExists(string name);
        Task<NetworkRegisterRequest> GetNetworkRegisterRequestAsync(int requestId);

        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedNetworkRegisterRequestAsync();
        Task<IEnumerable<NetworkRegisterRequest>> ListOpenNetworkRegisterRequestsAsync();

        Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalNetworkRegisterRequestsAsync();

        Task<IEnumerable<ApplicationUser>> ListNetworkAdminsAsync(int networkId);

        Task<NetworkRegisterRequest> GetNetworkRegisterRequestByUserEmailAsync(string email);

        Task<OrganisationNetwork> UpdateOrganisationNetworkAsync(OrganisationNetwork organisationNetwork);

        Task<Network> CreateNetworkAsync(Network network);
        Task<Network> UpdateNetworkAsync(NetworkDTO networkDto);
        Task<NetworkUser> AddUserToNetworkAsync(string userId, int networkId);
        Task RemoveUserFromNetworkAsync(string userId, int networkId);

        Task<NetworkRegisterRequest> AddNetworkRegisterRequestAsync(NetworkRegisterRequest request);
        Task DeleteNetworkRegisterRequestAsync(NetworkRegisterRequest request);
        Task<bool> AddBiobankToNetworkAsync(int biobankId, int networkId, string biobankExternalID, bool approve);
        Task RemoveBiobankFromNetworkAsync(int biobankId, int networkId);

        Task<NetworkRegisterRequest> UpdateNetworkRegisterRequestAsync(NetworkRegisterRequest request);

    }
}
