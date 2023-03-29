using System.Threading.Tasks;
using System.Linq;
using Biobanks.Data;
using Biobanks.Entities.Shared.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class StorageTemperatureService : ReferenceDataCrudService<StorageTemperature>
    {
        public StorageTemperatureService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets
                .Include(x => x.MaterialDetails)
                .CountAsync(x => x.MaterialDetails.Any(y => y.StorageTemperatureId == id));

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.StorageTemperatureId == id);
    }
}

