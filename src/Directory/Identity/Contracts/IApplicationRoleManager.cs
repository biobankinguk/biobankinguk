using System.Linq;

namespace Biobanks.Identity.Contracts
{
    public interface IApplicationRoleManager<TRole, TIdentityResult>
    {
        IQueryable<TRole> Roles { get; }
    }
}
