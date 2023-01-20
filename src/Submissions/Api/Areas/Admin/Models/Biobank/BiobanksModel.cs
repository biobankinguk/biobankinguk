using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Biobank;

public class BiobanksModel
{
  public ICollection<BiobankModel> Biobanks { get; set; }
}
