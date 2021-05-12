using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Analytics.Core.Contracts;

namespace Biobanks.Analytics.Core
{
    public class BiobankReadService : IBiobankReadService
    {
        private BiobanksDbContext _db;

        public BiobankReadService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IList<string>> GetOrganisationNames()
        {
            return (await ListBiobanksAsync()).Select(x => x.Name).ToList();
        }

        public async Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true)
        {
            return await _db.Organisations.Where(x => x.Name.Contains(wildcard) && (includeSuspended || x.IsSuspended == false)).ToListAsync();
        }


        public async Task<IList<string>> GetOrganisationExternalIds()
        {
            return (await ListBiobanksAsync()).Select(x => x.OrganisationExternalId).ToList();
        }

    }
}
