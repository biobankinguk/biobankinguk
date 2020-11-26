using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Data;

namespace Analytics.Services.Contracts
{
    public interface IBiobankWebService
    {
        Task<IEnumerable<string>> GetOrganisationExternalIds();
        Task<IEnumerable<string>> GetOrganisationNames();
        Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
    }
}