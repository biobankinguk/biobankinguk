﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;
using System;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public class RegistrationReasonService : ReferenceDataService<RegistrationReason>
    {
        public RegistrationReasonService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.OrganisationRegistrationReasons.CountAsync(x => x.RegistrationReasonId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.OrganisationRegistrationReasons.AnyAsync(x => x.RegistrationReasonId == id);
    }
}