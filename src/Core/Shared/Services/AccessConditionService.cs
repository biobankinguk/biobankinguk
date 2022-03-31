using Biobanks.Aggregator.Services;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AccessConditionService : ReferenceDataService

    {
        private readonly BiobanksDbContext _db;
        public AccessConditionService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AccessCondition> Query()
            => Query().Include(x => x.Organisations);      

    }
}
