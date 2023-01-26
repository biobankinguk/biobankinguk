using Biobanks.Data;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
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

