

using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
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
