using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Header;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobanks.Directory.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public HeaderViewComponent(IMemoryCache cache, IWebHostEnvironment env, IConfiguration config)
    {
        _cache = cache;
        _env = env;
        _config = config;
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
        var _wordPressUrl = _config["WordPressMenuUrl"];

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
