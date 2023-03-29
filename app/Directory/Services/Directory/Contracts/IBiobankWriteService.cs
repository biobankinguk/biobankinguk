using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Models.Directory;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IBiobankWriteService
    {

        Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference);
        Task RemoveLogoAsync(int organisationId);

        Task AddBiobankServicesAsync(IEnumerable<OrganisationServiceOffering> services);
        Task DeleteBiobankServiceAsync(int biobankId, int serviceId);

        Task UpdateOrganisationAnnualStatisticAsync(int organisationId, int statisticId, int? value, int year);
        Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons);
        Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId);

    }
}
