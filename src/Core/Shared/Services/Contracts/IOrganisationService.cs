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
    }
}