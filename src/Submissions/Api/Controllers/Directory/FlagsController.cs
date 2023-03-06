using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Biobanks.Submissions.Api.Controllers.Directory;

[Authorize(nameof(AuthPolicies.IsSuperUser))]
public class FlagsController : Controller
{
    private readonly IConfigService _configService;
    public FlagsController(IConfigService configService)
    {
        _configService = configService;
    }
    public async Task<ActionResult> Index()
    {

        return View((await _configService.ListBooleanFlags(BooleanConfigSelection.AllBooleanConfigs))
            .Select(x => new SiteConfigModel
            {
                Key = x.Key,
                Value = x.Value,
                Name = x.Name,
                Description = x.Description,
                ReadOnly = x.ReadOnly,
                IsFeatureFlag = x.IsFeatureFlag
            }));
    }

    [HttpPost]
    public async Task<ActionResult> UpdateFlagsConfig(IEnumerable<SiteConfigModel> values)
    {
        // Update Database Config
        await _configService.UpdateSiteConfigsAsync(
            values
                .OrderBy(x => x.Key)
                .Select(x => new Entities.Data.Config
                {
                    Key = x.Key,
                    Value = x.Value ?? "", // Store nulls as empty strings
                })
        );
        
        // Update current config cache
        await _configService.PopulateSiteConfigCache();
        
        return Ok(new
          {
            redirect = "UpdateFlagsConfigSuccess"
          });
    }

    public ActionResult UpdateFlagsConfigSuccess()
    {
        this.SetTemporaryFeedbackMessage("Flags configuration saved successfully.", FeedbackMessageType.Success);
        return RedirectToAction("FlagsConfig");
    }
    
}
