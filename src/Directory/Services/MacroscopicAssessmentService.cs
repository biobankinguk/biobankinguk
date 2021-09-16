using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class MacroscopicAssessmentService : ReferenceDataService<MacroscopicAssessment>
    {
        public MacroscopicAssessmentService(BiobanksDbContext db) : base (db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.MacroscopicAssessmentId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.MacroscopicAssessmentId == id);
    }
}
