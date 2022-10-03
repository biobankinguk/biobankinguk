﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding core version"
        , false)]
    public class AssociatedDataTypeGroupService : ReferenceDataService<AssociatedDataTypeGroup>
    {
        public AssociatedDataTypeGroupService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AssociatedDataTypeGroup> Query()
            => base.Query().Include(x => x.AssociatedDataTypes);

        public override async Task<int> GetUsageCount(int id)
            => await _db.AssociatedDataTypes.CountAsync(x => x.AssociatedDataTypeGroupId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.AssociatedDataTypes.AnyAsync(x => x.AssociatedDataTypeGroupId == id);
    }
}