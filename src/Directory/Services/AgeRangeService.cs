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


    //    public async Task<bool> AreAgeRangeBoundsNull(int id)
    //=> (await _ageRangeRepository.ListAsync(false, x => x.Id == id))
    //    .Where(x => string.IsNullOrEmpty(x.LowerBound) && string.IsNullOrEmpty(x.UpperBound)).Any();
    }
}
