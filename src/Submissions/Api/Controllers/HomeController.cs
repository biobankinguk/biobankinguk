using Biobanks.Directory.Data.Constants;
using Biobanks.Submissions.Api.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using System.Configuration;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public HomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET: Home
        [HttpGet]
        [SwaggerResponse(200)]

        public ActionResult Index()
        {
            var viewName = ConfigurationManager.AppSettings["AlternateHomepage"] ==  "true"
                ? "AltIndex" : "Index";

            var model = new HomepageContentModel
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
            };
            return new ViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                }
            };
        }
    }
}
