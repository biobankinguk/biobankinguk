using System.Linq;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Shared.Services
{
    public class AccessConditionService : ReferenceDataService<AccessCondition>

    {
        public AccessConditionService(ApplicationDbContext db) : base(db) { }

    protected override IQueryable<AccessCondition> Query()
            => Query().Include(x => x.Organisations);
    }
}
