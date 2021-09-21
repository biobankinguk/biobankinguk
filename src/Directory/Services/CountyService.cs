using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class CountyService : ReferenceDataService<County>
    {
        public CountyService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<County> Query()
            => base.Query()
                .Include(x => x.Country)
                .Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await _db.Organisations.CountAsync(x => x.CountyId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Organisations.AnyAsync(x => x.CountyId == id);
    }
}
