using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Areas.Admin.Models;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Biobanks.Submissions.Api.Services.Directory;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
public class RegistrationDomainRulesController : Controller
{
  private readonly RegistrationDomainService _registrationDomainService;
  
  public RegistrationDomainRulesController(RegistrationDomainService registrationDomainService)
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
