using Biobanks.Identity.Data;
using Biobanks.Identity.Data.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Biobanks.Identity.Services
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>
    {
        public ApplicationRoleStore(UserStoreDbContext context) : base(context)
        {
            DisposeContext = false;
        }
    }
}
