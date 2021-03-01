using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Biobanks.Web
{
    public partial class Startup
    {
        private static void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider()
                {
                    OnApplyRedirect = ctx =>
                    {   // Do not redirect 401 Unauthorised responses for WebApi
                        // This assumes every api route starts with /api
                        if (!ctx.Request.Path.StartsWithSegments(new PathString("/api")))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                },
                ReturnUrlParameter = "returnUrl",
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(20).Add(TimeSpan.FromSeconds(30)) //an extra half minute on the cookie allows the client side stuff to not break ;)
            });
        }
    }
}