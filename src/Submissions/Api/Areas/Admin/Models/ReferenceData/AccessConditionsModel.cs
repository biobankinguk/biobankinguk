using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class AccessConditionsModel
{
  public ICollection<ReadAccessConditionsModel> AccessConditions;
}
