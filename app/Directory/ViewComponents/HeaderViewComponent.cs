using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Biobanks.Submissions.Api.Models.Header;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Biobanks.Submissions.Api.Config;

namespace Biobanks.Submissions.Api.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;

    public HeaderViewComponent(IMemoryCache cache, IWebHostEnvironment env)
    {
        _cache = cache;
        _env = env;
    }

    public IViewComponentResult Invoke()
    {
        var _headerPath = Path.Combine(_env.ContentRootPath, "Settings/header.json");
        // Base Model From header.json
        var json = File.ReadAllText(_headerPath);
        var model = JsonConvert.DeserializeObject<HeaderModel>(json);

        // Build Navigation From Two Sources
        var userActions = UserActions();
        var wordpressItems = WordPress();

        model.NavigationItems = wordpressItems.Concat(userActions);

        return View(model);
    }

    private IEnumerable<NavItemModel> UserActions()
    {
        var _navPath = Path.Combine(_env.ContentRootPath, "Settings/navigation.json");

        var json = File.ReadAllText(_navPath);
        var navMenuItems = JsonConvert.DeserializeObject<IEnumerable<NavItemModel>>(json);

        return navMenuItems;
    }

    private IEnumerable<NavItemModel> WordPress()
    {
        var _wordPressUrl = ConfigurationManager.AppSettings["WordPressMenuUrl"];

        // Attempt to use cached data
        var wordPressMenuItems = _cache.Get<IEnumerable<NavItemModel>>(CacheKey.WordpressNavItems);

        // If nothing in the cache; try and get it from source and populate the cache
        if (wordPressMenuItems is null)
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
                        _cache.Set(CacheKey.WordpressNavItems, wordPressMenuItems, TimeSpan.FromDays(1));
                    }
                }
                finally
                {
                    httpClient?.Dispose();
                }
            }
        }

        // If we're STILL null for some reason, default to an empty list
        return wordPressMenuItems ?? Enumerable.Empty<NavItemModel>();
    }
}
