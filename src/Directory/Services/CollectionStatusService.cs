using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public class CollectionStatusService : ReferenceDataService<CollectionStatus>
    {
        public CollectionStatusService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.Collections.CountAsync(x => x.CollectionStatusId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionStatusId == id);
    }
}
