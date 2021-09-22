﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AccessConditionService : ReferenceDataService<AccessCondition>
    {
        public AccessConditionService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AccessCondition> Query()
            => base.Query().Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await _db.Collections.CountAsync(x => x.AccessConditionId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Collections.AnyAsync(x => x.AccessConditionId == id);
    }
}