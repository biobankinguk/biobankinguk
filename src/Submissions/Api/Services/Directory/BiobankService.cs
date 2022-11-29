using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankService : IBiobankService
    {
        private readonly IGenericEFRepository<OrganisationServiceOffering> _organisationServiceOfferingRepository;

        public BiobankService(IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository)
        {
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
        }

        public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId)
            => await _organisationServiceOfferingRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId,
                null,
                x => x.ServiceOffering);
    }
}
