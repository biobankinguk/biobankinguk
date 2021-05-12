using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Analytics.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IList<string>> GetOrganisationExternalIds();
        Task<IList<string>> GetOrganisationNames();
        Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
    }
}