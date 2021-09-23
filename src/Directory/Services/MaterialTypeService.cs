using Biobanks.Directory.Data;
using Biobanks.Entities.Shared.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class MaterialTypeService : ReferenceDataService<MaterialType>
    {
        public MaterialTypeService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<MaterialType> Query()
            => base.Query()
                .Include(x => x.ExtractionProcedures)
                .Include(x => x.MaterialTypeGroups);

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.MaterialTypeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.MaterialTypeId == id);
    }
}