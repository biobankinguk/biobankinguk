using Biobanks.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Middleware
{
    public class UpdateLastLoginMiddleware
    {
        private readonly RequestDelegate _next;

        //Updates the users last login datetime upon sucessful auth
        public UpdateLastLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                using (var dbContext = context.RequestServices.GetRequiredService<BiobanksDbContext>())
                {
                    var user = dbContext.Users.Where(u => u.UserName == context.User.Identity.Name).FirstOrDefault();
              //      user.LastLogin = DateTime.Now;
                    dbContext.Update(user);
                    dbContext.SaveChanges();
                }
            }
            //call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    public static class UpdateLastLoginMiddlewareExtension
        {
            public static IApplicationBuilder UseLoginUpdater(this IApplicationBuilder builder)
                => builder.UseMiddleware<UpdateLastLoginMiddleware>();
        }
}
