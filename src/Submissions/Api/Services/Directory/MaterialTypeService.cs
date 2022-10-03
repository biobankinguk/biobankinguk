using Biobanks.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Directory
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