using Biobanks.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Middleware
{
    public class DirectoryLoginMiddleware
    {
        private readonly RequestDelegate _next;

        //Updates the users last login datetime upon sucessful auth
        public DirectoryLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User != null && context.User.Identity.IsAuthenticated)
            {
                var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
                var user = dbContext.Users.FirstOrDefault(u => u.UserName == context.User.Identity.Name);
                user.LastLogin = DateTime.Now;
                dbContext.Update(user);
                dbContext.SaveChanges();
                
            }

            //call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    public static class DirectoryLoginMiddlewareExtension
    {
      public static IApplicationBuilder UseDirectoryLogin(this IApplicationBuilder builder)
        => builder.UseMiddleware<DirectoryLoginMiddleware>();
    }
}
