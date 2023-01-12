using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankService : IBiobankService
    {
        private readonly IGenericEFRepository<Organisation> _organisationRepository;
        private readonly IGenericEFRepository<OrganisationServiceOffering> _organisationServiceOfferingRepository;
        private readonly IGenericEFRepository<OrganisationUser> _organisationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public BiobankService(
            IGenericEFRepository<Organisation> organisationRepository,
            IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository,
            IGenericEFRepository<OrganisationUser> organisationUserRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            _organisationRepository = organisationRepository;
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
            _organisationUserRepository = organisationUserRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId)
            => await _organisationServiceOfferingRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId,
                null,
                x => x.ServiceOffering);

        public async Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId)
        {
          var adminIds = (await _organisationUserRepository.ListAsync(
              false,
              x => x.OrganisationId == biobankId))
              .Select(x => x.OrganisationUserId);

          return _userManager.Users.AsNoTracking().Where(x => adminIds.Contains(x.Id));
        }
        
        public async Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId)
            => (await _organisationRepository.ListAsync(
                    false,
                    x => x.OrganisationId == biobankId,
                    null,
                    x => x.Funders))
                .Select(x => x.Funders)
                .FirstOrDefault();
    }
}
