using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Auth;

public static class AuthCookieConfiguration
{
  public static readonly string ProfileCookieName = ".Biobank.Profile";
  public static readonly string IdentityCookieName = ".Biobank.Identity";

  public static readonly CookieOptions ProfileCookieOptions = new()
  {
    // Most actual COOKIE settings between Profile and Identity Cookies should match

    IsEssential = true,
    SameSite = SameSiteMode.Lax, // In Identity, `Lax` is default

    // This is the key difference to IdentityCookie; this one is INTENDED to be read by JS :)
    HttpOnly = false, // In Identity `true` is default
  };

  public static readonly Action<CookieAuthenticationOptions> IdentityCookieOptions =
  o =>
  {
    o.Cookie.Name = IdentityCookieName;

    o.Cookie.IsEssential = true;

    o.ExpireTimeSpan = TimeSpan.FromDays(30);

    o.SlidingExpiration = true;

      // While we are using Cookie Auth,
      // all requests to the backend are expected to be headless,
      // so interactive auth flow isn't helpful to us
    o.Events.OnRedirectToLogin = context =>
    {
      context.Response.Headers["Location"] = context.RedirectUri;
      context.Response.StatusCode = 401;
      return Task.CompletedTask;
    };

    o.Events.OnRedirectToAccessDenied = context =>
    {
      context.Response.Headers["Location"] = context.RedirectUri;
      context.Response.StatusCode = 403;
      return Task.CompletedTask;
    };
  };
}
