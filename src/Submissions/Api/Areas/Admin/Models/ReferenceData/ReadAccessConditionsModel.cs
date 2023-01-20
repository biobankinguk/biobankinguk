namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class ReadAccessConditionsModel : AccessConditionsModel
{
  //Sum of all Collections and Capabilities
  public int AccessConditionCount { get; set; }
}
