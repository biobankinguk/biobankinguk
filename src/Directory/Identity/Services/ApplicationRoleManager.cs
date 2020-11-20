using Directory.Identity.Contracts;
using Directory.Identity.Data.Entities;
using Directory.Identity.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Directory.Identity.Services
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>, IApplicationRoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<UserStoreDbContext>()));
        }
    }
}
