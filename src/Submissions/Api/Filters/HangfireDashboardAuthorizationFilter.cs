using Biobanks.Submissions.Api.Auth;
using Hangfire.Dashboard;

namespace Biobanks.Submissions.Api.Filters
{
    /// <inheritdoc />
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <inheritdoc />
        public bool Authorize(DashboardContext ctx)
        {
            // TODO: How does this even work?
            return ctx.GetHttpContext().User.IsInRole(CustomRoles.SuperAdmin);
        }
    }
}
