using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class CollectionStatusService : ReferenceDataCrudService<CollectionStatus>
    {
        public CollectionStatusService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.Collections.CountAsync(x => x.CollectionStatusId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionStatusId == id);
    }
}
