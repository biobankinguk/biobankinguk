﻿using Microsoft.AspNetCore.Identity;
using System;

namespace Biobanks.Submissions.Api.Auth.Entities
{
    //temporarily in place to replace framework identity
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }
        public DateTime? LastLogin { get; set;}
    }
}
