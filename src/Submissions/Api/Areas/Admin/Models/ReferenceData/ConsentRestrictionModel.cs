using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class ConsentRestrictionModel
{
  public ICollection<ReadConsentRestrictionModel> ConsentRestrictions;
}