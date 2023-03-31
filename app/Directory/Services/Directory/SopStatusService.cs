using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
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
