using System.Security.Claims;
using System.Security.Principal;
using Biobanks.Identity;

namespace Biobanks.Web.Extensions
{
    public static class IPrincipalExtensions
    {
        public static ApplicationUserPrincipal ToApplicationUserPrincipal(this IPrincipal user) => new ApplicationUserPrincipal(user as ClaimsPrincipal);
    }
}
