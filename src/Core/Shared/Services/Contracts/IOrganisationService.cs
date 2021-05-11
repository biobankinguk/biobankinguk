using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<IEnumerable<Organisation>> List();
    }
}