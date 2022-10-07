using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Entities.Data;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IMaterialService : IReferenceDataService<MaterialType>
    {
        //Task<int> GetExtractionProcedureMaterialDetailsCount(string id);

        Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id, bool onlyDisplayable = false);
    }
}

