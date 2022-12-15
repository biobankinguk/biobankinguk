using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AccessConditionService : ReferenceDataService<AccessCondition>

    {
        public AccessConditionService(BiobanksDbContext db) : base(db) { }


    public override async Task<int> GetUsageCount(int id)
        => await _db.Collections.CountAsync(x => x.AccessConditionId == id);


    protected override IQueryable<AccessCondition> Query()
            => Query().Include(x => x.Organisations);
    }
}
