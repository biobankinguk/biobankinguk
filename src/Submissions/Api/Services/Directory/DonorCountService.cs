﻿using System;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class DonorCountService : ReferenceDataService<DonorCount>
    {
        public DonorCountService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets.CountAsync(x => x.DonorCountId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.SampleSets.AnyAsync(x => x.DonorCountId == id);

        public override async Task<DonorCount> Update(DonorCount entity)
        {
            var existing = await base.Update(entity);
            existing.LowerBound = entity.LowerBound;
            existing.UpperBound = entity.UpperBound;
            await _db.SaveChangesAsync();

            return existing;
        }
    }
}
