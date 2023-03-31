using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class DonorCountService : ReferenceDataCrudService<DonorCount>
    {
        public DonorCountService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets.CountAsync(x => x.DonorCountId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.SampleSets.AnyAsync(x => x.DonorCountId == id);

        public override async Task<DonorCount> Update(DonorCount entity)
        {
            var existing = await base.Update(entity);
            existing.LowerBound = entity.LowerBound;
            existing.UpperBound = entity.UpperBound;
            await _db.SaveChangesAsync();

            return existing;
        }
    }
}

