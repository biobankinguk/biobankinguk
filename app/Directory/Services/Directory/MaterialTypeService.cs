using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Services.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
  public class MaterialTypeService : ReferenceDataCrudService<MaterialType>, IMaterialTypeService
  {
    public MaterialTypeService(ApplicationDbContext db) : base(db)
    {
    }

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

    public async Task<int> GetMaterialTypeMaterialDetailCount(int id)
      => await _db.MaterialDetails.CountAsync(x => x.MaterialTypeId == id);

    public async Task<bool> IsMaterialTypeAssigned(int id)
      => await _db.OntologyTerms
        .Include(x => x.MaterialTypes)
        .Where(x => x.SnomedTag != null && x.SnomedTag.Value == SnomedTags.ExtractionProcedure)
        .AnyAsync(x => x.MaterialTypes.Any(y => y.Id == id));

    public IEnumerable<string> ExtractDistinctMaterialTypes(Collection collection)
    {
      if (collection == null) throw new ArgumentNullException(nameof(collection));

      return collection.SampleSets
        .SelectMany(x => x.MaterialDetails)
        .Select(x => x.MaterialType?.Value)
        .Distinct();
    }
  }
}
