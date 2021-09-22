using Biobanks.Directory.Data;
using Biobanks.Entities.Shared.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class StorageTemperatureService : ReferenceDataService<StorageTemperature>
    {
        public StorageTemperatureService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.StorageTemperatureId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.StorageTemperatureId == id);
    }
}
