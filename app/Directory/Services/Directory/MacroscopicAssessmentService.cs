using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class MacroscopicAssessmentService : ReferenceDataCrudService<MacroscopicAssessment>
    {
        public MacroscopicAssessmentService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.MacroscopicAssessmentId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.MacroscopicAssessmentId == id);
    }
}

