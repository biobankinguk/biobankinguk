﻿using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace Biobanks.Submissions.Api.Services.Directory
{
    public class CountryService : ReferenceDataService<Country>
    {
        public CountryService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<Country> Query()
            => base.Query()
                .Include(x => x.Counties)
                .Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await _db.Counties.CountAsync(x => x.CountryId == id) + await _db.Organisations.CountAsync(x => x.CountryId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Counties.AnyAsync(x => x.CountryId == id) || await _db.Organisations.AnyAsync(x => x.CountryId == id);
    }
}