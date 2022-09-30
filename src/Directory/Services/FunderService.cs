using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public class FunderService : ReferenceDataService<Funder>
    {
        public FunderService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<Funder> Query()
            => base.Query()
                .Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.Organisations)
                .CountAsync();

        public override async Task<bool> IsInUse(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.Organisations)
                .AnyAsync();
    }
}
