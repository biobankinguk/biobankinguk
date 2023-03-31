using System;
using System.Threading.Tasks;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Home;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Controllers.Directory
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IConfigService _configService;
        private readonly SitePropertiesOptions _sitePropertiesOptions;

        public HomeController(
          IConfigService configService, 
          IOptions<SitePropertiesOptions> sitePropertiesOptions)
        {
            _configService = configService;
            _sitePropertiesOptions = sitePropertiesOptions.Value;
        }

        // GET: Home
        public async Task<IActionResult> Index()
        {
            var viewName = _sitePropertiesOptions.AlternateHomepage == "true"
                ? "AltIndex" : "Index";

            return View(viewName, new HomepageContentModel
            {
                Title = await _configService.GetSiteConfigValue(ConfigKey.HomepageTitle, "", true),
                SearchTitle = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchTitle, "", true),
                SearchSubTitle = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchSubTitle, "", true),
                ResourceRegistration = await _configService.GetSiteConfigValue(ConfigKey.HomepageResourceRegistration, "", true),
                ResourceRegistration2 = await _configService.GetSiteConfigValue(ConfigKey.HomepageResourceRegistration2, "", true),
                NetworkRegistration = await _configService.GetSiteConfigValue(ConfigKey.HomepageNetworkRegistration, "", true),
                NetworkRegistration2 = await _configService.GetSiteConfigValue(ConfigKey.HomepageNetworkRegistration2, "", true),
                RequireSamplesCollected = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchRadioSamplesCollected, "", true),
                AccessExistingSamples = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchRadioAccessSamples, "", true),
                FinalParagraph = await _configService.GetSiteConfigValue(ConfigKey.HomepageFinalParagraph, "", true),
                ResourceRegistrationButton = await _configService.GetSiteConfigValue(ConfigKey.RegisterBiobankTitle, "", true),
                NetworkRegistrationButton = await _configService.GetSiteConfigValue(ConfigKey.RegisterNetworkTitle, "", true)
            });
        }

        public ActionResult Cookies() => View();

        public ViewResult Contact() => View();

        public ActionResult FeedbackMessageAjax(string message, string type, bool html = false)
        {
            this.SetTemporaryFeedbackMessage(
                message,

                ((Func<FeedbackMessageType>)(() =>
                {
                    switch (type?.ToLower() ?? "")
                    {
                        case "success":
                            return FeedbackMessageType.Success;
                        case "danger":
                            return FeedbackMessageType.Danger;
                        case "warning":
                            return FeedbackMessageType.Warning;
                        default:
                            return FeedbackMessageType.Info;
                    }
                }))(),

                html);

            return PartialView("_FeedbackMessage");
        }
    }
}
