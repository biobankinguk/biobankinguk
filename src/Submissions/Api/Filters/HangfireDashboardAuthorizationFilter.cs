using Biobanks.Submissions.Api.Auth;

using Hangfire.Dashboard;

using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Filters
{
    /// <inheritdoc />
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <inheritdoc />
        public bool Authorize(DashboardContext context)
        {
            if (context.GetHttpContext().User.IsInRole(CustomRoles.SuperAdmin))
            {
                return true;
            }
            else
            {
                return Task.Run(async () => await Challenge(context.GetHttpContext())).Result;
            }
        }

        private async Task<bool> Challenge(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");

            await context.Response.WriteAsync("Authentication is required.");

            return false;
        }
    }
}
