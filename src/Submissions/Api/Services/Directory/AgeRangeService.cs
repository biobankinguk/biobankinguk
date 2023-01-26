using System;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
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

