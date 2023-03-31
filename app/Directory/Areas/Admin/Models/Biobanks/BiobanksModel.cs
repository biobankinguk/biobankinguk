using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Admin.Models.Biobanks;

public class BiobanksModel
{
  public ICollection<BiobankModel> Biobanks { get; set; }
}
