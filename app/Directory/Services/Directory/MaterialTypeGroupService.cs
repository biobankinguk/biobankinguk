using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class MaterialTypeGroupService : ReferenceDataCrudService<MaterialTypeGroup>
    {
        public MaterialTypeGroupService(ApplicationDbContext db) : base(db) { }

        protected override IQueryable<MaterialTypeGroup> Query()
            => base.Query().Include(x => x.MaterialTypes);

        public override async Task<int> GetUsageCount(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.MaterialTypes)
                .CountAsync();

        public override async Task<bool> IsInUse(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.MaterialTypes)
                .AnyAsync();
    }
}
