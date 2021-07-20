using Biobanks.Entities.Data;
using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Biobanks.Services.ConfigService;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "SuperUser")]
    public class FlagsController : ApplicationBaseController
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
                    .Select(x => new Config
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
            SetTemporaryFeedbackMessage("Flags configuration saved successfully.", FeedbackMessageType.Success);
            return RedirectToAction("FlagsConfig");
        }
    }
}