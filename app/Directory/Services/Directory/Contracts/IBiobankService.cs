using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IBiobankService
    {
        Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferings(int biobankId);
        Task<IEnumerable<ApplicationUser>> ListBiobankAdmins(int biobankId);
        Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIds(int modelBiobankId);
        Task<string> GetUnusedTokenByUser(string biobankUserId);
        Task<IEnumerable<Funder>> ListBiobankFunders(int biobankId);
        Task AddBiobankServiceOfferings(IEnumerable<OrganisationServiceOffering> services);
        Task DeleteBiobankServiceOffering(int biobankId, int serviceId);
        Task UpdateOrganisationAnnualStatistic(int organisationId, int statisticId, int? value, int year);
        Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons);
        Task DeleteBiobankRegistrationReason(int organisationId, int registrationReasonId);
    }
}
