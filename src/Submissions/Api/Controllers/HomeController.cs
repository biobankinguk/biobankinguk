using Biobanks.Directory.Data.Constants;
using Biobanks.Submissions.Api.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;

namespace Biobanks.Submissions.Api.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public HomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET: Home
        public IActionResult Index()
        {
            var viewName = ConfigurationManager.AppSettings["AlternateHomepage"] ==  "true"
                ? "AltIndex" : "Index";

            return View(viewName, new HomepageContentModel
            {

                Title = (string)_memoryCache.Get(ConfigKey.HomepageTitle),
                SearchTitle = (string)_memoryCache.Get(ConfigKey.HomepageSearchTitle),
                SearchSubTitle = (string)_memoryCache.Get(ConfigKey.HomepageSearchSubTitle),
                ResourceRegistration = (string)_memoryCache.Get(ConfigKey.HomepageResourceRegistration),
                ResourceRegistration2 = (string)_memoryCache.Get(ConfigKey.HomepageResourceRegistration2),
                NetworkRegistration = (string)_memoryCache.Get(ConfigKey.HomepageNetworkRegistration),
                NetworkRegistration2 = (string)_memoryCache.Get(ConfigKey.HomepageNetworkRegistration2),
                RequireSamplesCollected = (string)_memoryCache.Get(ConfigKey.HomepageSearchRadioSamplesCollected),
                AccessExistingSamples = (string)_memoryCache.Get(ConfigKey.HomepageSearchRadioAccessSamples),
                FinalParagraph = (string)_memoryCache.Get(ConfigKey.HomepageFinalParagraph),
                ResourceRegistrationButton = (string)_memoryCache.Get(ConfigKey.RegisterBiobankTitle),
                NetworkRegistrationButton = (string)_memoryCache.Get(ConfigKey.RegisterNetworkTitle)
            });
        }
        public ActionResult Cookies() => View();

        public ViewResult Contact() => View();
    }
}
