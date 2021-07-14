using System;
using System.Collections.Generic;
using System.Web;
using Hangfire.Dashboard;

namespace Biobanks.Web.Filters
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.IsInRole("SuperUser");
        }
    }
}