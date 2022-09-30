using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Biobanks.Entities.Data;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Dto;
using System;

namespace Biobanks.Services.Contracts
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public interface IBiobankWriteService
    {
        Task AddSampleSetAsync(SampleSet sampleSet);
        Task UpdateSampleSetAsync(SampleSet sampleSet);
        Task DeleteSampleSetAsync(int id);

        Task AddCapabilityAsync(CapabilityDTO capability, IEnumerable<CapabilityAssociatedData> associatedData);
        Task UpdateCapabilityAsync(CapabilityDTO capability, IEnumerable<CapabilityAssociatedData> associatedData);
        Task DeleteCapabilityAsync(int id);

        Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference);
        Task RemoveLogoAsync(int organisationId);

        Task AddBiobankServicesAsync(IEnumerable<OrganisationServiceOffering> services);
        Task DeleteBiobankServiceAsync(int biobankId, int serviceId);

        Task UpdateOrganisationAnnualStatisticAsync(int organisationId, int statisticId, int? value, int year);
        Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons);
        Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId);

    }
}
