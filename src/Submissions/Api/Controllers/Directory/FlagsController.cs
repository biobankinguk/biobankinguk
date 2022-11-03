using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Controllers.Directory;

public class FlagsController : Controller
{
    private readonly IConfigService _configService;
    public FlagsController(IConfigService configService)
    {
        _configService = configService;
    }
    public async Task<ActionResult> FlagsConfig()
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
    public async Task<JsonResult> UpdateFlagsConfig(IEnumerable<SiteConfigModel> values)
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


        // Invalidate current config (Refreshed in SiteConfigAttribute filter)
        HttpContext.Application["Config"] = null;

        return Json(new
        {
            success = true,
            redirect = "UpdateFlagsConfigSuccess"
        });
    }

    public ActionResult UpdateFlagsConfigSuccess()
    {
        TemporaryFeedbackMessageExtensions.SetTemporaryFeedbackMessage(this, "Flags configuration saved successfully.", FeedbackMessageType.Success, false);
        return RedirectToAction("FlagsConfig");
    }
    
}