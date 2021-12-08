using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AssociatedDataProcurementTimeframeService : ReferenceDataService<AssociatedDataProcurementTimeframe>
    {
        public AssociatedDataProcurementTimeframeService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.CollectionAssociatedDatas.CountAsync(x => x.AssociatedDataProcurementTimeframeId == id)
             + await _db.CapabilityAssociatedDatas.CountAsync(x => x.AssociatedDataProcurementTimeframeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.CollectionAssociatedDatas.AnyAsync(x => x.AssociatedDataProcurementTimeframeId == id)
            || await _db.CapabilityAssociatedDatas.AnyAsync(x => x.AssociatedDataProcurementTimeframeId == id);

        public override async Task<AssociatedDataProcurementTimeframe> Update(AssociatedDataProcurementTimeframe entity)
        {
            var existing = await base.Update(entity);
            existing.DisplayValue = entity.DisplayValue;
            await _db.SaveChangesAsync();

            return existing;
        }
    }
}