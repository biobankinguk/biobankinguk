using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AgeRangeService : ReferenceDataService<AgeRange>
    {
        public AgeRangeService(BiobanksDbContext db) : base(db) { }

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
