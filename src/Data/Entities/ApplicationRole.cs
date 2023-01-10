using System;
using Microsoft.AspNetCore.Identity;

namespace Biobanks.Data.Entities;

public class ApplicationRole : IdentityRole<string>
{
    public ApplicationRole()
    {
        this.Id = Guid.NewGuid().ToString();
    }

    public ApplicationRole(string name)
        : this()
    {
        this.Name = name;
    }
    
}
