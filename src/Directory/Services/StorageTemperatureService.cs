using Biobanks.Directory.Data;
using Biobanks.Entities.Shared.ReferenceData;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public class StorageTemperatureService : ReferenceDataService<StorageTemperature>
    {
        public StorageTemperatureService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets
                .Include(x => x.MaterialDetails)
                .CountAsync(x => x.MaterialDetails.Any(y => y.StorageTemperatureId == id));

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.StorageTemperatureId == id);
    }
}
