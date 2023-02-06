using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Areas.Admin.Models.RegistrationDomainRules;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.AspNetCore.Authorization;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Services.Directory.Contracts;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

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
