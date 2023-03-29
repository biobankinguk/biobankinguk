using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Areas.Admin.Models.RegistrationDomainRules;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]

public class RegistrationDomainRulesController : Controller
{
  private readonly IRegistrationDomainService _registrationDomainService;
  
  public RegistrationDomainRulesController(IRegistrationDomainService registrationDomainService)
  {
    _registrationDomainService = registrationDomainService;
  }
  
  public async Task<ActionResult> BlockAllowList()
  {
    var rules = (await _registrationDomainService.ListRules())
      .Select(x => new RegistrationDomainRuleModel
      {
        Id = x.Id,
        DateModified = x.DateModified,
        RuleType = x.RuleType,
        Source = x.Source,
        Value = x.Value

      })
      .ToList();

    return View(new BlockAllowListModel
    {
      RegistrationDomainRules = rules
    });
  }
  
}
