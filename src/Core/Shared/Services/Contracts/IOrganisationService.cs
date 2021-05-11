using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<IEnumerable<Organisation>> List(bool includeSuspended = true);
    }
}