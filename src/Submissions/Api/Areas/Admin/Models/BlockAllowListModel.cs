using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models;

public class BlockAllowListModel
{
  public ICollection<RegistrationDomainRuleModel> RegistrationDomainRules;
}
