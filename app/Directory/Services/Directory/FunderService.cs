using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class FunderService : ReferenceDataCrudService<Funder>
    {
        public FunderService(ApplicationDbContext db) : base(db) { }

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

