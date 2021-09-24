using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AssociatedDataTypeService : ReferenceDataService<AssociatedDataType>
    {
        public AssociatedDataTypeService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AssociatedDataType> Query()
            => base.Query().Include(x => x.AssociatedDataTypeGroup);

        public override async Task<int> GetUsageCount(int id)
            => await _db.CollectionAssociatedDatas.CountAsync(x => x.AssociatedDataTypeId == id)
             + await _db.CapabilityAssociatedDatas.CountAsync(x => x.AssociatedDataTypeId == id);
        
        public override async Task<bool> IsInUse(int id)
            => await _db.CollectionAssociatedDatas.AnyAsync(x => x.AssociatedDataTypeId == id)
            || await _db.CapabilityAssociatedDatas.AnyAsync(x => x.AssociatedDataTypeId == id);
    }
}