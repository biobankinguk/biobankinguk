using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data;
using Biobanks.Identity.Data.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Biobanks.Identity.Services
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>, IApplicationRoleManager<ApplicationRole, IdentityResult>
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
