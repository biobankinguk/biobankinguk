using Directory.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<List<string>> GetOrganisationNames();
        Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
    }
}