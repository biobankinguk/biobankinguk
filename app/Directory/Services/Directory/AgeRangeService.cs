using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class AgeRangeService : ReferenceDataCrudService<AgeRange>
    {
        public AgeRangeService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets.CountAsync(x => x.AgeRangeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.SampleSets.AnyAsync(x => x.AgeRangeId == id);

        public override async Task<AgeRange> Update(AgeRange entity)
        {
            var existing = await base.Update(entity);
            existing.UpperBound = entity.UpperBound;
            existing.LowerBound = entity.LowerBound;
            await _db.SaveChangesAsync();

            return existing;
        }
    }
}

