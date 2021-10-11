using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class SopStatusService : ReferenceDataService<SopStatus>
    {
        public SopStatusService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.Networks.CountAsync(x => x.SopStatusId == id);
            
        public override async Task<bool> IsInUse(int id)
            => await _db.Networks.AnyAsync(x => x.SopStatusId == id);
    }
}