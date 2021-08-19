using Biobanks.Entities.Data;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface INetworkService
    {
        Task<Network> Get(int networkId);
        Task<Network> GetByName(string networkName);

        Task<IEnumerable<Network>> List();
        Task<IEnumerable<Network>> ListByOrganisationId(int organisationId);
        Task<IEnumerable<Network>> ListByUserId(string userId);

        Task<IEnumerable<OrganisationNetwork>> ListOrganisationNetworks(IEnumerable<int> organisationIds);
        Task<IEnumerable<OrganisationNetwork>> ListOrganisationNetworks(int biobankId);
        Task<OrganisationNetwork> GetOrganisationNetwork(int biobankId, int networkId);

        Task<bool> HasActiveRegistrationRequest(string name);
        Task<NetworkRegisterRequest> GetRegistrationRequest(int requestId);

        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequestsByUserId(string userId);
        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequests();

        Task<IEnumerable<NetworkRegisterRequest>> ListOpenRegistrationRequests();
        Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalRegistrationRequests();

        Task<IEnumerable<ApplicationUser>> ListAdmins(int networkId);

        Task<NetworkRegisterRequest> GetRegistrationRequestByEmail(string email);

        Task<OrganisationNetwork> UpdateOrganisationNetwork(OrganisationNetwork organisationNetwork);

        Task<Network> Create(Network network);
        Task<Network> Update(Network network);
        Task<NetworkUser> AddNetworkUser(string userId, int networkId);
        Task RemoveNetworkUser(string userId, int networkId);

        Task<NetworkRegisterRequest> AddRegistrationRequest(NetworkRegisterRequest request);
        Task DeleteRegistrationRequest(NetworkRegisterRequest request);
        Task<bool> AddOrganisationToNetwork(int biobankId, int networkId, string biobankExternalID, bool approve);
        Task RemoveOrganisationFromNetwork(int biobankId, int networkId);

        Task<NetworkRegisterRequest> UpdateRegistrationRequest(NetworkRegisterRequest request);

    }
}
