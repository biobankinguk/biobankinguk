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

        Task<Network> Get(int networkId);
        Task<Network> GetByName(string networkName);

        Task<IEnumerable<Network>> List();
        Task<IEnumerable<Network>> ListByOrganisationId(int organisationId);

        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId);

        List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId);

        List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId);

        Task<bool> HasActiveRegistrationRequest(string name);
        Task<NetworkRegisterRequest> GetRegistrationRequest(int requestId);

        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequests();
        Task<IEnumerable<NetworkRegisterRequest>> ListOpenRegistrationRequests();

        Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalRegistrationRequests();

        Task<IEnumerable<ApplicationUser>> ListNetworkAdminsAsync(int networkId);

        Task<NetworkRegisterRequest> GetRegistrationRequestByEmail(string email);

        Task<OrganisationNetwork> UpdateOrganisationNetworkAsync(OrganisationNetwork organisationNetwork);

        Task<Network> CreateNetworkAsync(Network network);
        Task<Network> UpdateNetworkAsync(NetworkDTO networkDto);
        Task<NetworkUser> AddUserToNetwork(string userId, int networkId);
        Task RemoveUserFromNetworkAsync(string userId, int networkId);

        Task<NetworkRegisterRequest> AddRegistrationRequest(NetworkRegisterRequest request);
        Task DeleteRegistrationRequest(NetworkRegisterRequest request);
        Task<bool> AddOrganisationToNetwork(int biobankId, int networkId, string biobankExternalID, bool approve);
        Task RemoveBiobankFromNetworkAsync(int biobankId, int networkId);

        Task<NetworkRegisterRequest> UpdateRegistrationRequest(NetworkRegisterRequest request);

    }
}
