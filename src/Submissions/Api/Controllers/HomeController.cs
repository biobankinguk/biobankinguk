using Biobanks.Directory.Data.Constants;
using Biobanks.Submissions.Api.Models.Home;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IConfigService _configService;

        public HomeController(IConfigService configService)
        {
            _configService = configService;
        }

        // GET: Home
        public async Task<IActionResult> IndexAsync()
        {
            var viewName = ConfigurationManager.AppSettings["AlternateHomepage"] ==  "true"
                ? "AltIndex" : "Index";

            return View(viewName, new HomepageContentModel
            {
                Title = await _configService.GetSiteConfigValue(ConfigKey.HomepageTitle, true, ""),
                SearchTitle = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchTitle, true, ""),
                SearchSubTitle = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchSubTitle, true, ""),
                ResourceRegistration = await _configService.GetSiteConfigValue(ConfigKey.HomepageResourceRegistration, true, ""),
                ResourceRegistration2 = await _configService.GetSiteConfigValue(ConfigKey.HomepageResourceRegistration2, true, ""),
                NetworkRegistration = await _configService.GetSiteConfigValue(ConfigKey.HomepageNetworkRegistration, true, ""),
                NetworkRegistration2 = await _configService.GetSiteConfigValue(ConfigKey.HomepageNetworkRegistration2, true, ""),
                RequireSamplesCollected = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchRadioSamplesCollected, true, ""),
                AccessExistingSamples = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchRadioAccessSamples, true, ""),
                FinalParagraph = await _configService.GetSiteConfigValue(ConfigKey.HomepageFinalParagraph ,true,""),
                ResourceRegistrationButton = await _configService.GetSiteConfigValue(ConfigKey.RegisterBiobankTitle, true, ""),
                NetworkRegistrationButton = await _configService.GetSiteConfigValue(ConfigKey.RegisterNetworkTitle, true, "")
            });
        }
       public ActionResult Cookies() => View();
    }
}
