using Biobanks.Web.Models.Header;
using Directory.Data.Caching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Mvc;
using Castle.Core.Internal;
using System.Net;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class HeaderController : Controller
    {
        private readonly string _headerPath = HostingEnvironment.MapPath(@"~/App_Config/header.json");
        private readonly string _navPath = HostingEnvironment.MapPath(@"~/App_Config/navigation.json");
        private readonly string _wordPressUrl = ConfigurationManager.AppSettings["WordPressMenuUrl"];

        private readonly ICacheProvider _cacheProvider;

        public HeaderController(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public ActionResult Header()
        {
            // Base Model From header.json
            var json = System.IO.File.ReadAllText(_headerPath);
            var model = JsonConvert.DeserializeObject<HeaderModel>(json);

            // Build Navigation From Two Sources
            var userActions = UserActions();
            var wordpressItems = WordPress();

            model.NavigationItems = wordpressItems.Concat(userActions);

            return PartialView("_BBHeader", model);
        }

        private IEnumerable<NavItemModel> UserActions()
        {
            var json = System.IO.File.ReadAllText(_navPath);
            var navMenuItems = JsonConvert.DeserializeObject<IEnumerable<NavItemModel>>(json);

            return navMenuItems;
        }

        private IEnumerable<NavItemModel> WordPress()
        {
            // Default as empty list
            var wordPressMenuItems = Enumerable.Empty<NavItemModel>();

            // Attempt to use cached data
            try
            {
                wordPressMenuItems = _cacheProvider.Retrieve<IEnumerable<NavItemModel>>(CacheKeys.WordpressNavItems);
            }
            catch
            {
                if (!_wordPressUrl.IsNullOrEmpty())
                {
                    var httpClient = new HttpClient();

                    try
                    {
                        var response = httpClient.GetAsync(_wordPressUrl).Result;
                        var result = response.Content.ReadAsStringAsync().Result;

                        if (response.IsSuccessStatusCode && !result.IsNullOrEmpty())
                        {
                            // Parse relevant json field
                            var jsonResult = JsonConvert.DeserializeAnonymousType(result, new
                            {
                                items = Enumerable.Empty<NavItemModel>()
                            });

                            wordPressMenuItems = jsonResult.items;

                            // Cache Data
                            _cacheProvider.Store<IEnumerable<NavItemModel>>(CacheKeys.WordpressNavItems, wordPressMenuItems, TimeSpan.FromDays(1));
                        }
                    }
                    finally
                    {
                        httpClient?.Dispose();
                    }
                }
            }

            return wordPressMenuItems;
        }
    }
}