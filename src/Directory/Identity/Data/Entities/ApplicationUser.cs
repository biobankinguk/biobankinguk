using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Identity.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Biobanks.Identity.Data.Entities
{
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();

            // Add any custom User properties/code here
            HistoricalPasswords = new List<HistoricalPassword>();
        }

        public string Name { get; set; }
        public DateTime? LastLogin { get; set; }

        public virtual IList<HistoricalPassword> HistoricalPasswords { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(IApplicationUserManager<ApplicationUser, string, IdentityResult> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
