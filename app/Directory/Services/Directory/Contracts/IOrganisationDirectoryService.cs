using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Models.Directory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IOrganisationDirectoryService
    {
        /// <summary>
        /// Get all untracked Organisation which match the given parameter filters
        /// </summary>
        /// <param name="name">Organisation name filter, matches all organisation that contains name</param>
        /// <param name="includeSuspended">Results to include Organgisations that have been suspended, true by default</param>
        /// <returns>An Enumerable of all applicable untracked Organisations</returns>
        Task<IEnumerable<Organisation>> List(string name = "", bool includeSuspended = true);

        /// <summary>
        /// Get all untracked Organisation which match the given parameter filters
        /// </summary>
        /// <param name="name">Organisation name filter, matches all organisation that contains name</param>
        /// <param name="includeSuspended">Results to include Organgisations that have been suspended, true by default</param>
        /// <returns>
        /// An Enumerable of all applicable untracked Organisations, including properties:
        /// <list type="bullet">
        /// <item>Collections</item>
        /// <item>DiagnosisCapabilities</item>
        /// <item>OrganisationUsers</item>
        /// </list>
        /// </returns>
        Task<IEnumerable<Organisation>> ListForActivity(string name = "", bool includeSuspended = true);

        /// <summary>
        /// Get all untracked Organisation which match by Anonymous Identifer
        /// </summary>
        /// <param name="organisationAnonymousIds">Collection of Organisation Annoymous Identifier to match against</param>
        /// <returns>An Enumerable of all applicable untracked Organisations</returns>
        Task<IEnumerable<Organisation>> ListByAnonymousIdentifiers(IEnumerable<Guid> organisationAnonymousIds);

        /// <summary>
        /// Get all untracked Organisation which match by Organisation External Id
        /// </summary>
        /// <param name="organisationExternalId">Collection of Organisation External Id to match against</param>
        /// <returns>An Enumerable of all applicable untracked Organisations</returns>
        Task<IEnumerable<Organisation>> ListByExternalIds(IList<string> organisationExternalId);

        /// <summary>
        /// Get all untracked Organisation which match are part of the given Network
        /// </summary>
        /// <param name="networkId">The Id of the Network to match against</param>
        /// <returns>An Enumerable of all applicable untracked Organisations</returns>
        Task<IEnumerable<Organisation>> ListByNetworkId(int networkId);

        /// <summary>
        /// Get all untracked Organisation which are under the control by the given User
        /// <param name="userId">The Id of the User to match against</param>
        /// </summary>
        /// <returns>An Enumerable of all applicable untracked Organisations</returns>
        Task<IEnumerable<Organisation>> ListByUserId(string userId);

        /// <summary>
        /// Get the untracked Organisation for a given Id
        /// </summary>
        /// <returns>The Organisation of given Id. Otherwise null</returns>
        Task<Organisation> Get(int biobankId);
        /// <summary>
        /// Get the untracked Organisation by a given Id, with default Access Condition and 
        /// Collection Type for bulk submissions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Organisation> GetForBulkSubmissions(int id);

        /// <summary>
        /// Get the untracked Organisation for a given External Id
        /// </summary>
        /// <returns>The Organisation of given External Id. Otherwise null</returns>
        Task<Organisation> GetByExternalId(string externalId);

        /// <summary>
        /// Get the untracked Organisation for a given Organisation Name
        /// </summary>
        /// <returns>The Organisation of name. Otherwise null</returns>
        Task<Organisation> GetByName(string organisationName);

        /// <summary>
        /// Create a new Organisation
        /// </summary>
        /// <returns>The newly created Organisation with assigned Id</returns>
        Task<Organisation> Create(OrganisationDTO organisationDto);

        /// <summary>
        /// Update an exisiting Organisation
        /// </summary>
        /// <returns>The updated Organisation reference</returns>
        Task<Organisation> Update(Organisation organisation);

        /// <summary>
        /// Delete an exisiting Organisation
        /// </summary>
        Task Delete(int organisationId);

        /// <summary>
        /// Checks if a Registration Request exists for an Organisation of given name
        /// </summary>
        /// <param name="name">Name of the Organisation</param>
        /// <returns>true - If an Open or Accepted request exists for the given Organisation</returns>
        Task<bool> RegistrationRequestExists(string name);

        /// <summary>
        /// Checks if there is any associated API Credentials for the given Organisation
        /// </summary>
        /// <param name="organisationId">Internal Id of the Organisation</param>
        /// <returns>true - There is at least one set of API Credenitals for the given Organisation</returns>
        Task<bool> IsApiClient(int organisationId);

        /// <summary>
        /// Checks if the Organisation is suspended
        /// </summary>
        Task<bool> IsSuspended(int organisationId);

        /// <summary>
        /// Suspends an Organisation
        /// </summary>
        Task<Organisation> Suspend(int organisationId);

        /// <summary>
        /// Unsuspends an Organisation
        /// </summary>
        Task<Organisation> Unsuspend(int organisationId);

        /// <summary>
        /// List all untracked, accepted Organisation Registration Requests
        /// </summary>
        /// <returns>Enumerable of all accepted registration requests</returns>
        Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedRegistrationRequests();        
        
        /// <summary>
        /// List all untracked, accepted Organisation Registration Requests for a given user
        /// </summary>
        /// <param name="userId">The Id of the user</param>
        /// <returns>Enumerable of all accepted <see cref="OrganisationRegisterRequest"/> for a given user</returns>
        Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedRegistrationRequestsByUserId(string userId);

        /// <summary>
        /// List all untracked, open Organisation Registration Requests
        /// </summary>
        /// <returns>Enumerable of all accepted registration requests</returns>
        Task<IEnumerable<OrganisationRegisterRequest>> ListOpenRegistrationRequests();

        /// <summary>
        /// List all untracked, historic Organisation Registration Requests. Historic requests are either accepted or declined
        /// </summary>
        /// <returns>Enumerable of all historic registration requests</returns>
        Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalRegistrationRequests();

        /// <summary>
        /// Get the Registration Request by it's Id
        /// </summary>
        /// <returns>A Registration Request if it exists, otherwise null</returns>
        Task<OrganisationRegisterRequest> GetRegistrationRequest(int requestId);

        /// <summary>
        /// Get the Registration Request by the email of a user in the request
        /// </summary>
        /// <param name="email">The email of the user in the request</param>
        /// <returns>A Registration Request if it exists, otherwise null</returns>
        Task<OrganisationRegisterRequest> GetRegistrationRequestByEmail(string email);

        /// <summary>
        /// Get the Registration Request by the name of the Organisation in the request
        /// </summary>
        /// <param name="name">The name of the request's Organisation</param>
        /// <returns>A Registration Request if it exists, otherwise null</returns>
        Task<OrganisationRegisterRequest> GetRegistrationRequestByName(string name);

        /// <summary>
        /// Add a new Registration Request for a new Organisation
        /// </summary>
        /// <returns>The newly created Registration Request</returns>
        Task<OrganisationRegisterRequest> AddRegistrationRequest(OrganisationRegisterRequest request);

        /// <summary>
        /// Update an exisiting Registration Request for a new Organisation
        /// </summary>
        /// <returns>The updated Registration Request</returns>
        Task<OrganisationRegisterRequest> UpdateRegistrationRequest(OrganisationRegisterRequest request);

        /// <summary>
        /// Delete an exisiting Registration Request for a new Organisation
        /// </summary>
        Task RemoveRegistrationRequest(OrganisationRegisterRequest request);

        /// <summary>
        /// Adds an exisiting Sser to an exisiting Organisation
        /// </summary>
        /// <param name="userId">The Id of the User</param>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <returns>A new OrganisationUser relationship</returns>
        Task<OrganisationUser> AddUserToOrganisation(string userId, int organisationId);

        /// <summary>
        /// Remove an exisiting Sser to an exisiting Organisation
        /// </summary>
        /// <param name="userId">The Id of the User</param>
        /// <param name="organisationId">The Id of the Organisation</param>
        Task RemoveUserFromOrganisation(string userId, int organisationId);

        /// <summary>
        /// Adds an exisiting Funder to an exisiting Organisation
        /// </summary>
        /// <param name="funderId">The Id of the Funder being added</param>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <returns>true - If successful in adding Funder</returns>
        Task AddFunder(int funderId, int organisationId);

        /// <summary>
        /// Removes an exisiting Funder to an exisiting Organisation
        /// </summary>
        /// <param name="funderId">The Id of the Funder being added</param>
        /// <param name="organisationId">The Id of the Organisation</param>
        Task RemoveFunder(int funderId, int organisationId);

        /// <summary>
        /// Get the last active user for a given Organisation
        /// </summary>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <returns>
        /// The User that was last online, that is part of the given Organisation. 
        /// Otherwise null, if no Users or Organisation exists
        /// </returns>
        Task<ApplicationUser> GetLastActiveUser(int organisationId);

        /// <summary>
        /// Generates a Credential (ID, Secret) pair for authenticating against the API as the given Organisation
        /// </summary>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <param name="clientName">Optional client name, null by default</param>
        /// <returns>KeyValuePair (ID, Secret). The generated ID is permenant, but the secret can be regenerated</returns>
        Task<KeyValuePair<string, string>> GenerateNewApiClient(int organisationId, string clientName = null);

        /// <summary>
        /// Regenerates the API Secret for a given Organisation
        /// </summary>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <returns>KeyValuePair (ID, Secret) with newly regenerated Secret</returns>
        Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int organisationId);

        /// <summary>
        /// Lists the reasons for an Organisations registration
        /// </summary>
        /// <returns>An untracked Enumerable of Organisation Registration Reasons</returns>
        Task<IEnumerable<OrganisationRegistrationReason>> ListRegistrationReasons(int organisationId);

        /// <summary>
        /// Whether this Organisation make use of the Publication feature. If so, the Directory will attempt to source
        /// relevant publications associated with this Organisation
        /// </summary>
        Task<bool> UsesPublications(int organisationId);
    }
}
