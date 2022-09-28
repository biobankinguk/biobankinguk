using Microsoft.AspNetCore.Identity;
using System;

namespace Biobanks.Submissions.Api.Auth.Entities
{
    //temporarily in place to replace framework identity
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLogin { get; set;}
    }
}
