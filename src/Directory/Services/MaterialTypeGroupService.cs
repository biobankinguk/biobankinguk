﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Shared.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public class MaterialTypeGroupService : ReferenceDataService<MaterialTypeGroup>
    {
        public MaterialTypeGroupService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<MaterialTypeGroup> Query()
            => base.Query().Include(x => x.MaterialTypes);

        public override async Task<int> GetUsageCount(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.MaterialTypes)
                .CountAsync();

        public override async Task<bool> IsInUse(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.MaterialTypes)
                .AnyAsync();
    }
}