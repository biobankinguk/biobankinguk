using System.Collections.Generic;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.RegistrationDomainRules;

public class BlockAllowListModel
{
  public ICollection<RegistrationDomainRuleModel> RegistrationDomainRules;
}
