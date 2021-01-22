using Biobanks.Web.Controllers;
using Biobanks.Web.Models.Shared;
using Biobanks.Directory.Services.Contracts;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Filters
{
    public class SiteConfigAttribute : FilterAttribute, IActionFilter
    {

        public IBiobankReadService BiobankReadService { get; set; }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            using (MiniProfiler.Current.Step("Filter: SiteConfigAttribute"))
            {
                var app = context.HttpContext.Application;

                if (app["Config"] == null)
                {
                    app["Config"] = BiobankReadService.ListSiteConfigs().ToDictionary(x => x.Key, x => x.Value);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {}

    }

}