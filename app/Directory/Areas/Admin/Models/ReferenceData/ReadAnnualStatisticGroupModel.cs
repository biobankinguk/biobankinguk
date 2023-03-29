using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class ReadAnnualStatisticGroupModel : AnnualStatisticGroupModel
{
  //Count where AnnualStatisticGroup is referenced
  public int AnnualStatisticGroupCount { get; set; }
}
