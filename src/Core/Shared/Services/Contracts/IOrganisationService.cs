using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Shared.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<IEnumerable<Organisation>> List();
    }
}