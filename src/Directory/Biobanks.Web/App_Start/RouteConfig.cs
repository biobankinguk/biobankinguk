﻿using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Biobanks.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*robotstxt}", new {robotstxt=@"(.*/)?robots.txt(/.*)?"});

            routes.MapRoute(
                name: "ContentPage",
                url: "Pages/{slug}",
                defaults: new { controller = "PagesAdmin", action = "ContentPage"}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
