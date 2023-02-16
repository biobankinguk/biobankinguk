using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
  public class MaterialTypeService : ReferenceDataCrudService<MaterialType>, IMaterialTypeService
  {
    public MaterialTypeService(ApplicationDbContext db) : base(db) { }

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


    public IEnumerable<string> ExtractDistinctMaterialTypes(Collection collection)
    {
      if (collection == null) throw new ArgumentNullException(nameof(collection));

      var x = collection.SampleSets.SelectMany(x => x.MaterialDetails);

      return collection.SampleSets
          .SelectMany(x => x.MaterialDetails)
          .Select(x => x.MaterialType?.Value)
          .Distinct();
    }
  }
}
