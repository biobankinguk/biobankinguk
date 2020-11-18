using Directory.Data.Entities;
using Microsoft.EntityFrameworkCore;
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
        private PublicationDbContext _ctx;

        public BiobankReadService(
            PublicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "",
                bool includeSuspended = true)
            => await _ctx.Organisations.Where(
                x => x.Name.Contains(wildcard) &&
                (includeSuspended || x.IsSuspended == false)).ToListAsync();

        public async Task<IList<string>> GetOrganisationNames()
            => (await ListBiobanksAsync()).Select(x => x.Name).ToList();

    }
}
