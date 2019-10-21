using System;
using Microsoft.AspNetCore.Identity;

namespace Common.Data.Identity
{
    public class DirectoryUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; } = string.Empty;

        public DateTimeOffset? LastLogin { get; set; }
    }
}
