using Biobanks.Directory.Data.Caching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting.Internal;
using Biobanks.Submissions.Api.Models.Header;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Biobanks.Submissions.Api.Controllers.Submissions
{
    [AllowAnonymous]
    public class HeaderController : ControllerBase
    {

        private readonly ICacheProvider _cacheProvider;
        private IWebHostEnvironment _hostEnvironment;

        public HeaderController(ICacheProvider cacheProvider, IWebHostEnvironment environment)
        {
            _cacheProvider = cacheProvider;
            _hostEnvironment = environment;

        }

        public ActionResult Header()
        {
            var _headerPath = Path.Combine(_hostEnvironment.WebRootPath, @"~/App_Config/header.json");
            // Base Model From header.json
            var json = System.IO.File.ReadAllText(_headerPath);
            var model = JsonConvert.DeserializeObject<HeaderModel>(json);

            // Build Navigation From Two Sources
            var userActions = UserActions();
            var wordpressItems = WordPress();

            model.NavigationItems = wordpressItems.Concat(userActions);

            return new PartialViewResult { 
                ViewName = "_BBHeader", 
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(),new ModelStateDictionary())
                {
                    Model = model
                }
            };
        }

        private IEnumerable<NavItemModel> UserActions()
        {
            var _navPath = Path.Combine(_hostEnvironment.WebRootPath, @"~/App_Config/navigation.json");

            var json = System.IO.File.ReadAllText(_navPath);
            var navMenuItems = JsonConvert.DeserializeObject<IEnumerable<NavItemModel>>(json);

            return navMenuItems;
        }

        private IEnumerable<NavItemModel> WordPress()
        {
            var _wordPressUrl = ConfigurationManager.AppSettings["WordPressMenuUrl"];

        // Default as empty list
            var wordPressMenuItems = Enumerable.Empty<NavItemModel>();

            // Attempt to use cached data
            try
            {
                wordPressMenuItems = _cacheProvider.Retrieve<IEnumerable<NavItemModel>>(CacheKeys.WordpressNavItems);
            }
            catch
            {
                if (!string.IsNullOrEmpty(_wordPressUrl))
                {
                    var httpClient = new HttpClient();

                    try
                    {
                        var response = httpClient.GetAsync(_wordPressUrl).Result;
                        var result = response.Content.ReadAsStringAsync().Result;

                        if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(result))
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