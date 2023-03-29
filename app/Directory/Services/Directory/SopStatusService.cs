using Biobanks.Data;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class SopStatusService : ReferenceDataCrudService<SopStatus>
    {
        public SopStatusService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.Networks.CountAsync(x => x.SopStatusId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.Networks.AnyAsync(x => x.SopStatusId == id);
    }
}
