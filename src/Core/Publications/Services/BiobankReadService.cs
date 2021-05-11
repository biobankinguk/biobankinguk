using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Biobanks.Publications.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        private BiobanksDbContext _db;

        public BiobankReadService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "",
                bool includeSuspended = true)
            => await _db.Organisations.Where(
                x => x.Name.Contains(wildcard) &&
                (includeSuspended || x.IsSuspended == false)).ToListAsync();

        public async Task<IList<string>> GetOrganisationNames()
            => (await ListBiobanksAsync()).Select(x => x.Name).ToList();
        
        //Gets Publications by OrganisationId, if Accepted is true, if the pub has no previously synced annotations and if last sync was over a month ago
        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
            => await _db.Publications.Where(x => x.OrganisationId == biobankId && x.Accepted == true)
            .Where(a => a.AnnotationsSynced == null || a.AnnotationsSynced < DateTime.Today.AddMonths(-1)).ToListAsync();
    }
}
