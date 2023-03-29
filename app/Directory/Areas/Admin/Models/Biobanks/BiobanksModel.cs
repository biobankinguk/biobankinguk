using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Biobanks;

public class BiobanksModel
{
  public ICollection<BiobankModel> Biobanks { get; set; }
}
