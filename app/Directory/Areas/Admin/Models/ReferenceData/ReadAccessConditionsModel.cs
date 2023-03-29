using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class ReadAccessConditionsModel : AccessConditionModel
{
  //Sum of all Collections and Capabilities
  public int AccessConditionCount { get; set; }
}
