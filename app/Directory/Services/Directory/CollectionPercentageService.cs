using System;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class CollectionPercentageService : ReferenceDataCrudService<CollectionPercentage>
    {
        public CollectionPercentageService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.CollectionPercentageId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.CollectionPercentageId == id);

        public override async Task<CollectionPercentage> Update(CollectionPercentage entity)
        {
            var existing = await base.Update(entity);
            existing.LowerBound = entity.LowerBound;
            existing.UpperBound = entity.UpperBound;
            await _db.SaveChangesAsync();

            return existing;
        }
    }
}

