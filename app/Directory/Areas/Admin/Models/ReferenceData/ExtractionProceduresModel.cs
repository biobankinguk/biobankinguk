using System.Collections.Generic;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.ReferenceData;

public class ExtractionProceduresModel
{
  public ICollection<ReadExtractionProcedureModel> ExtractionProcedures { get; set; }

  public IEnumerable<MaterialType> MaterialTypes { get; set; }
  
}
