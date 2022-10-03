﻿using Biobanks.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class PreservationTypeService : ReferenceDataService<PreservationType>
    {
        public PreservationTypeService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<PreservationType> Query()
            => base.Query().Include(x => x.StorageTemperature);

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.PreservationTypeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.PreservationTypeId == id);

        public override async Task<PreservationType> Update(PreservationType entity)
        {
            var existing = await base.Update(entity);
            existing.StorageTemperature = entity.StorageTemperature;
            existing.StorageTemperatureId = entity.StorageTemperatureId;
            await _db.SaveChangesAsync();

            return existing;
        }
    }
}
