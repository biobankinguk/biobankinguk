using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Directory.Enums;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Controllers.Directory;

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
                .Select(x => new Data.Entities.Config
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
        return RedirectToAction("Index");
    }
    
}
