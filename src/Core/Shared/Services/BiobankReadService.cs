using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Biobanks.Publications.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        private BiobanksDbContext _db;

        public BiobankReadService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "",
                bool includeSuspended = true)
            => await _db.Organisations.Where(
                x => x.Name.Contains(wildcard) &&
                (includeSuspended || x.IsSuspended == false)).ToListAsync();
    }
}
