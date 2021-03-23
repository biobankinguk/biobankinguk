using System;
using System.Collections.Generic;
using System.Web;
using Hangfire.Dashboard;

namespace Biobanks.Web.Filters
{
    [Obsolete]
    public class HangFireAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            return HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.IsInRole("SuperUser");
        }
    }
}