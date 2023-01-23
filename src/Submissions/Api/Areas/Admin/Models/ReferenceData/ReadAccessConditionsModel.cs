namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class ReadAccessConditionsModel : Biobanks.Submissions.Api.Models.Shared.AccessConditionModel
{
  //Sum of all Collections and Capabilities
  public int AccessConditionCount { get; set; }
}
