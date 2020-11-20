using System.Linq;

namespace Directory.Identity.Contracts
{
    public interface IApplicationRoleManager<TRole>
    {
        IQueryable<TRole> Roles { get; }
    }
}
