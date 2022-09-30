using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class CollectionTypeService : ReferenceDataService<CollectionType>
    {
        public CollectionTypeService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<CollectionType> Query()
            => base.Query().Include(x => x.Organisations);

        public override async Task<int> GetUsageCount(int id)
            => await _db.Collections.CountAsync(x => x.CollectionTypeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionTypeId == id);
    }
}