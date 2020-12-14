using Biobanks.Common.Auth;
using Hangfire.Dashboard;

namespace Biobanks.SubmissionApi.Filters
{
    /// <inheritdoc />
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <inheritdoc />
        public bool Authorize(DashboardContext ctx)
        {
            return ctx.GetHttpContext().User.IsInRole(CustomRoles.SuperAdmin);
        }
    }
}
