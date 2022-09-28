using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Auth.Entities
{
    //temporarily in place to replace framework identity
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLogin { get; set;}
        public virtual IList<HistoricalPassword> HistoricalPasswords { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(IApplicationUserManager<ApplicationUser, string, Microsoft.AspNetCore.Identity.IdentityResult> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
