using Directory.Data.Entities;
using Directory.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Publications.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Publications.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        private readonly IGenericEFRepository<Organisation> _organisationRepository;

        public BiobankReadService(
            IGenericEFRepository<Organisation> organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "",
                bool includeSuspended = true)
            => await _organisationRepository.ListAsync(
                false,
                x => x.Name.Contains(wildcard) &&
                (includeSuspended || x.IsSuspended == false));

        public async Task<List<string>> GetOrganisationNames()
            => (await ListBiobanksAsync()).Select(x => x.Name).ToList();

    }
}
