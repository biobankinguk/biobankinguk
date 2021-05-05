using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Analytics.Core.Contracts
{
    public interface IBiobankWebService
    {
        Task<IList<string>> GetOrganisationExternalIds();
        Task<IList<string>> GetOrganisationNames();
        Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
    }
}