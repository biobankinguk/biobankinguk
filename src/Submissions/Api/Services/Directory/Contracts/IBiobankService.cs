using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IBiobankService
    {
        Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId);
        Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId);
        Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int modelBiobankId);
        Task<string> GetUnusedTokenByUser(string biobankUserId);
        Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId);

    }
}
