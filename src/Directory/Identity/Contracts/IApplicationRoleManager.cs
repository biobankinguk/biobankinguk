using System.Linq;

namespace Biobanks.Identity.Contracts
{
    public interface IApplicationRoleManager<TRole>
    {
        IQueryable<TRole> Roles { get; }
    }
}
