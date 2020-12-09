using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Analytics.Services.Contracts;
using Analytics.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Services
{
    public class BiobankWebService : IBiobankWebService
    {
        private AnalyticsDbContext _ctx;

        public BiobankWebService(AnalyticsDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IList<string>> GetOrganisationNames()
        {
            return (await ListBiobanksAsync()).Select(x => x.Name).ToList();
        }

        public async Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true)
        {
            return await _ctx.Organisations.Where(x => x.Name.Contains(wildcard) && (includeSuspended || x.IsSuspended == false)).ToListAsync();
        }


        public async Task<IList<string>> GetOrganisationExternalIds()
        {
            return (await ListBiobanksAsync()).Select(x => x.OrganisationExternalId).ToList();
        }

    }
}
