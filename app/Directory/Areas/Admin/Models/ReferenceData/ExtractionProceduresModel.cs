using System.Collections.Generic;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class ExtractionProceduresModel
{
  public ICollection<ReadExtractionProcedureModel> ExtractionProcedures { get; set; }

  public IEnumerable<MaterialType> MaterialTypes { get; set; }
  
}
