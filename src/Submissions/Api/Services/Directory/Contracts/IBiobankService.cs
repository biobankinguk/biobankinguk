using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IBiobankService
    {
        Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId);
        Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId);

  }
}
