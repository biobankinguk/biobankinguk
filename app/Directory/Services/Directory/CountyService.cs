using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class CountyService : ReferenceDataCrudService<County>
    {
        public CountyService(ApplicationDbContext db) : base(db) { }

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

