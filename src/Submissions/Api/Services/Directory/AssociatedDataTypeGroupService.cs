using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
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

