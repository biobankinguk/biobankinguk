using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class SexService : ReferenceDataCrudService<Sex>
    {
        public SexService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets.CountAsync(x => x.SexId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.SampleSets.AnyAsync(x => x.SexId == id);
    }
}

