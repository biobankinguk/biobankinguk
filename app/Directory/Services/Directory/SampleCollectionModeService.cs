using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class SampleCollectionModeService : ReferenceDataCrudService<SampleCollectionMode>
    {
        public SampleCollectionModeService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.DiagnosisCapabilities.CountAsync(x => x.SampleCollectionModeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.DiagnosisCapabilities.AnyAsync(x => x.SampleCollectionModeId == id);
    }
}

