using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface INetworkService
    {
        /// <summary>
        /// Get all untracked Networks
        /// </summary>
        /// <returns>An Enumerable of all applicable untracked Networks</returns>
        Task<IEnumerable<Network>> List();

        /// <summary>
        /// Get all untracked Networks a given Organisation is a member of
        /// </summary>
        /// <param name="organisationId">The Id of the given Organisation</param>
        /// <returns>An Enumerable of all applicable untracked Networks</returns>
        Task<IEnumerable<Network>> ListByOrganisationId(int organisationId);

        /// <summary>
        /// Get all untracked Networks a given User is a member of
        /// </summary>
        /// <param name="userId">The Id of the given User</param>
        /// <returns>An Enumerable of all applicable untracked Networks</returns>
        Task<IEnumerable<Network>> ListByUserId(string userId);

        /// <summary>
        /// Get the untracked Networks a given Id
        /// </summary>
        /// <returns>The Network of given Id. Otherwise null</returns>
        Task<Network> Get(int networkId);

        /// <summary>
        /// Get the untracked Networks a given Id, used for indexing operations
        /// </summary>
        /// <returns>The Network of given Id. Otherwise null</returns>
        Task<Network> GetForIndexing(int networkId);

        /// <summary>
        /// Get the untracked Networks a given Name
        /// </summary>
        /// <returns>The Network of given Name. Otherwise null</returns>
        Task<Network> GetByName(string networkName);

        /// <summary>
        /// Create a new Network
        /// </summary>
        /// <returns>The newly created Network reference</returns>
        Task<Network> Create(NetworkDTO networkDto);

        /// <summary>
        /// Update an existing Network
        /// </summary>
        /// <returns>The updated Network reference</returns>
        Task<Network> Update(NetworkDTO networkDto);

        /// <summary>
        /// Get all untracked OrganisationNetwork relationships associated with a given Organisation
        /// </summary>
        /// <param name="organisationId">The Id of the applicable Organisations</param>
        /// <returns>An Enumerable of all applicable untracked OrganisationNetwork relationships</returns>
        Task<IEnumerable<OrganisationNetwork>> ListOrganisationNetworks(int organisationId);

        /// <summary>
        /// Get the untracked OrganisationNetwork for a given Organisation and Network
        /// </summary>
        /// <param name="organisationId">The Id of the applicable Organisations</param>
        /// <param name="networkId">The Id of the applicable Network</param>
        /// <returns>The untracked OrganisationNetwork relationships</returns>
        Task<OrganisationNetwork> GetOrganisationNetwork(int organisationId, int networkId);

        /// <summary>
        /// Update an exisiting OrganisationNetwork relationship
        /// </summary>
        /// <returns>The updated OrganisationNetwork relationships</returns>
        Task<OrganisationNetwork> UpdateOrganisationNetwork(OrganisationNetwork organisationNetwork);

        /// <summary>
        /// Whether the Network has an active registration request
        /// </summary>
        /// <param name="name">Name of the Network</param>
        /// <returns>true - There is an Active registration request for the Network</returns>
        Task<bool> HasActiveRegistrationRequest(string name);

        /// <summary>
        /// Gets the RegistrationRequest by its Id
        /// </summary>
        /// <param name="requestId">The Id of the Registration Request</param>
        /// <returns>The RegistrationRequest, if exists. Otherwise null</returns>
        Task<NetworkRegisterRequest> GetRegistrationRequest(int requestId);

        /// <summary>
        /// List all untracked, accepted Network Registration Requests for a given user 
        /// </summary>
        /// <param name="userId">The Id of the User</param>
        /// <returns>Enumerable of all registration requests for a given user</returns>
        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequestsByUserId(string userId);

        /// <summary>
        /// List all untracked, accepted Network Registration Requests
        /// </summary>
        /// <returns>Enumerable of all accepted registration requests</returns>
        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequests();

        /// <summary>
        /// List all untracked, open Network Registration Requests. Open requests are neither accepted nor declined
        /// </summary>
        /// <returns>Enumerable of all accepted registration requests</returns>
        Task<IEnumerable<NetworkRegisterRequest>> ListOpenRegistrationRequests();

        /// <summary>
        /// List all untracked, historic Network Registration Requests. Historic requests are either accepted or declined
        /// </summary>
        /// <returns>Enumerable of all accepted registration requests</returns>
        Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalRegistrationRequests();

        /// <summary>
        /// List all admin users of a network
        /// </summary>
        /// <param name="networkId">The Id of the Network</param>
        /// <returns>Enumerable of all accepted registration requests</returns>
        Task<IEnumerable<ApplicationUser>> ListAdmins(int networkId);

        /// <summary>
        /// Get a Network Registration Request by a given user email
        /// </summary>
        /// <param name="email">User email address</param>
        /// <returns>The open registration request associated with the given user email address. Otherwise null</returns>
        Task<NetworkRegisterRequest> GetRegistrationRequestByEmail(string email);

        /// <summary>
        /// Create a new Network Registration Request
        /// </summary>
        /// <returns>The created NetworkRegisterRequest reference</returns>
        Task<NetworkRegisterRequest> AddRegistrationRequest(NetworkRegisterRequest request);

        /// <summary>
        /// Update an exisiting Network Registration Request
        /// </summary>
        /// <returns>The updated NetworkRegisterRequest reference</returns>
        Task<NetworkRegisterRequest> UpdateRegistrationRequest(NetworkRegisterRequest request);

        /// <summary>
        /// Add an Organisation to a Network
        /// </summary>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <param name="networkId">The Id of the Network</param>
        /// <param name="organisationExternalId">The external Id of the Organisation</param>
        /// <param name="approve">Whether the request should be automatically be approved</param>
        /// <returns>true - Successfully added the Organisation to the Network</returns>
        Task<bool> AddOrganisationToNetwork(int organisationId, int networkId, string organisationExternalId, bool approve);

        /// <summary>
        /// Remove an Organisation from a Network
        /// </summary>
        Task RemoveOrganisationFromNetwork(int biobankId, int networkId);

        /// <summary>
        /// Add a new user to a Network
        /// </summary>
        /// <returns>The created NetworkUser relationship</returns>
        Task<NetworkUser> AddNetworkUser(string userId, int networkId);

        /// <summary>
        /// Remove a user from a Network
        /// </summary>
        Task RemoveNetworkUser(string userId, int networkId);
    }
}
