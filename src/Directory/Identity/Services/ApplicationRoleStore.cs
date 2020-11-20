using Directory.Identity.Data;
using Directory.Identity.Data.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Directory.Identity.Services
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>
    {
        public ApplicationRoleStore(UserStoreDbContext context) : base(context)
        {
            DisposeContext = false;
        }
    }
}
