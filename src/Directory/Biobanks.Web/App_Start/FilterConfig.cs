using System.Web.Mvc;
using Biobanks.Web.App_Start;
using Biobanks.Web.Filters;
using StackExchange.Profiling.Mvc;

namespace Biobanks.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ProfilingActionFilter()); // MiniProfiler Controller Filter
            filters.Add(new AiHandleErrorAttribute());
            filters.Add(new SiteConfigAttribute());
            filters.Add(new UserAuthenticateAttribute());
            filters.Add(new BiobanksAuthenticateAttribute());
            filters.Add(new RequireHttpsAttribute());
        }
    }
}
