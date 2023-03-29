using System;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class AccessConditionService : ReferenceDataCrudService<AccessCondition>
    {
        public AccessConditionService(ApplicationDbContext db) : base(db) { }

        protected override IQueryable<AccessCondition> Query()
            => base.Query().Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await _db.Collections.CountAsync(x => x.AccessConditionId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Collections.AnyAsync(x => x.AccessConditionId == id);
    }
}

