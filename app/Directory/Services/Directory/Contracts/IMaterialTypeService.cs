using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IMaterialTypeService : IReferenceDataCrudService<MaterialType>
    {
        Task<int> GetExtractionProcedureMaterialDetailsCount(string id);

        Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id, bool onlyDisplayable = false);
        IEnumerable<string> ExtractDistinctMaterialTypes(Collection collection);

  }
}

