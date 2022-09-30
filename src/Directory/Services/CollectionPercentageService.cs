﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding core version"
        , false)]
    public class CollectionPercentageService : ReferenceDataService<CollectionPercentage>
    {
        public CollectionPercentageService(BiobanksDbContext db) : base(db) { }

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
