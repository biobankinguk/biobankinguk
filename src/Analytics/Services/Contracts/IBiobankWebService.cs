using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Services.Contracts
{
    public interface IBiobankWebService
    {
        void Dispose();
        Task<List<string>> GetOrganisationExternalIds();
        Task<List<string>> GetOrganisationNames();
    }
}