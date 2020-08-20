using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analytics.Entities;
using Analytics.Repositories;

namespace Analytics.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        private readonly IGenericEFRepository<Organisation> _organisationRepository;

        public BiobankReadService(IGenericEFRepository<Organisation> organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<IEnumerable<Organisation>> ListBiobanksAsync()
        {
            return await _organisationRepository.ListAsync();
        }


        public async Task<Organisation> GetBiobankByExternalIdAsync(string externalId)
        {
            return (await _organisationRepository.ListAsync(
                false,
                x => x.OrganisationExternalId == externalId,
                null,
                null)).FirstOrDefault();
        }
    }
}

