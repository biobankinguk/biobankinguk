using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Biobanks.Publications.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        private BiobanksDbContext _ctx;

        public BiobankReadService(
            BiobanksDbContext ctx)
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
        
        //Gets Publications by OrganisationId, if Accepted is true, if the pub has no previously synced annotations and if last sync was over a month ago
        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
            => await _ctx.Publications.Where(x => x.OrganisationId == biobankId && x.Accepted == true)
            .Where(a => a.AnnotationsSynced == null || a.AnnotationsSynced < DateTime.Today.AddMonths(-1)).ToListAsync();
    }
}
