using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class CollectionStatusService : ReferenceDataService<CollectionStatus>
    {
        public CollectionStatusService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.Collections.CountAsync(x => x.CollectionStatusId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionStatusId == id);
    }
}
