using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Data;

namespace Analytics.Services.Contracts
{
    public interface IBiobankWebService
    {
        Task<IList<string>> GetOrganisationExternalIds();
        Task<IList<string>> GetOrganisationNames();
        Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
    }
}