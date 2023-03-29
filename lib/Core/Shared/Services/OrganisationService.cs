using Biobanks.Shared.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Biobanks.Shared.Services
{
    //TODO merge or resolve with OrganisationDirectoryService
    public class OrganisationService : IOrganisationService
    {
        private readonly ApplicationDbContext _db;

        public OrganisationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> Count()
            => await _db.Organisations.CountAsync();

        public async Task<IEnumerable<Organisation>> List()
            => await _db.Organisations.ToListAsync();

        public async Task<IEnumerable<string>> ListExternalIds()
            => await _db.Organisations.Select(x => x.OrganisationExternalId).ToListAsync();

        public async Task<Organisation> GetById(int organisationId)
            => await _db.Organisations
                .Include(x => x.AccessCondition)
                .Include(x => x.CollectionType)
                .FirstOrDefaultAsync(x => x.OrganisationId == organisationId);

        public async Task<Organisation> GetByExternalId(string externalId)
            => await _db.Organisations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganisationExternalId == externalId);

        public async Task<IEnumerable<Organisation>> ListByNetworkId(int networkId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Where(x => x.NetworkId == networkId && !x.Organisation.IsSuspended)
                .Select(x => x.Organisation)
                .ToListAsync();

    }
}
