using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.IdentityProvider.Data
{
    // This is a stub context, because the migration tooling needs it to be in the 
    // same namespace as the entry assembly.
    // It is correct that it adds nothing further to its Base class.

    public class IdpDbContext : IdentityDbContext<IdentityUser>
    {
        public IdpDbContext(DbContextOptions options) : base(options) { }
    }
}
