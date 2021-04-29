using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IList<string>> GetOrganisationNames();
        Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
        Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId);
    }
}