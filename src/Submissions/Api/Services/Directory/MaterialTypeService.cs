using Biobanks.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class MaterialTypeService : ReferenceDataService<MaterialType>, IMaterialTypeService
    {
        public MaterialTypeService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<MaterialType> Query()
            => base.Query()
                .Include(x => x.ExtractionProcedures)
                .Include(x => x.MaterialTypeGroups);

        public override async Task<int> GetUsageCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.MaterialTypeId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.MaterialDetails.AnyAsync(x => x.MaterialTypeId == id);

        public async Task<int> GetExtractionProcedureMaterialDetailsCount(string id)
            => await _db.MaterialDetails.CountAsync(x => x.ExtractionProcedureId == id);

        public Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id, bool onlyDisplayable = false)
            => Task.FromResult(_db.MaterialTypes
                .Where(x => x.Id == id)
                .Include(x => x.ExtractionProcedures)
                .FirstOrDefault()?
                .ExtractionProcedures
                .Where(x => x.DisplayOnDirectory || !onlyDisplayable));
    }
}
