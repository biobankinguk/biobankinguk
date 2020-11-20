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
                ReturnUrlParameter = "returnUrl",
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(20).Add(TimeSpan.FromSeconds(30)) //an extra half minute on the cookie allows the client side stuff to not break ;)
            });
        }
    }
}