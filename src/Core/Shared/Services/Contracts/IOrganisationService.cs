using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Shared.Services.Contracts
{
    //TODO merge or resolve with IOrganisationDirectoryService
    public interface IOrganisationService
    {
        Task<int> Count();

        Task<IEnumerable<Organisation>> List();

        Task<IEnumerable<string>> ListExternalIds();

        Task<Organisation> GetById(int organisationId);

        /// <summary>
        /// Get the untracked Organisation for a given External Id
        /// </summary>
        /// <returns>The Organisation of given External Id. Otherwise null</returns>
        Task<Organisation> GetByExternalId(string externalId);

        /// <summary>
        /// Get all untracked Organisation which match are part of the given Netowrk
        /// <param name="networkId">The Id of the Network to match against</param>
        /// <returns>An Enumerable of all applicable untracked Organisations</returns>
        Task<IEnumerable<Organisation>> ListByNetworkId(int networkId);
    }
}