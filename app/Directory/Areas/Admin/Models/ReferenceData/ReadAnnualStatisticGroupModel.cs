using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.ReferenceData;

public class ReadAnnualStatisticGroupModel : AnnualStatisticGroupModel
{
  //Count where AnnualStatisticGroup is referenced
  public int AnnualStatisticGroupCount { get; set; }
}
