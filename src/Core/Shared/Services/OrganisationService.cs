using Biobanks.Shared.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Biobanks.Shared.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly BiobanksDbContext _db;

        public OrganisationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<int> Count()
            => await _db.Organisations.CountAsync();

        public async Task<IEnumerable<Organisation>> List()
            => await _db.Organisations.ToListAsync();

        public async Task<IEnumerable<string>> ListExternalIds()
            => await _db.Organisations.Select(x => x.OrganisationExternalId).ToListAsync();

    }
}
