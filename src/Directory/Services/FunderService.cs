﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class FunderService : ReferenceDataService<Funder>
    {
        public FunderService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<Funder> Query()
            => base.Query()
                .Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.Organisations)
                .CountAsync();

        public override async Task<bool> IsInUse(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.Organisations)
                .AnyAsync();
    }
}