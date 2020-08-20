using Analytics.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Services
{
    public interface IBiobankReadService
    {
        Task<Organisation> GetBiobankByExternalIdAsync(string externalId);
        Task<IEnumerable<Organisation>> ListBiobanksAsync();
    }
}