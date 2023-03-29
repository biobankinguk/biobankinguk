using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class CountryService : ReferenceDataCrudService<Country>
    {
        public CountryService(ApplicationDbContext db) : base(db) { }

        protected override IQueryable<Country> Query()
            => base.Query()
                .Include(x => x.Counties)
                .Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await _db.Counties.CountAsync(x => x.CountryId == id) + await _db.Organisations.CountAsync(x => x.CountryId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Counties.AnyAsync(x => x.CountryId == id) || await _db.Organisations.AnyAsync(x => x.CountryId == id);
    }
}
