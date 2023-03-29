using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.ReferenceData;

public class ReadAccessConditionsModel : AccessConditionModel
{
  //Sum of all Collections and Capabilities
  public int AccessConditionCount { get; set; }
}
