using System.Collections.Generic;
using System.Threading.Tasks;

using Directory.Identity.Data.Entities;

namespace Directory.Identity.Contracts
{
    public interface IApplicationGroupManager<TIdentityResult, TUser, TRole>
    {
        IEnumerable<ApplicationGroup> Groups { get; }
        Task<TIdentityResult> CreateGroupAsync(ApplicationGroup group);
        TIdentityResult CreateGroup(ApplicationGroup group);
        TIdentityResult SetGroupRoles(string groupId, params string[] roleNames);
        Task<TIdentityResult> SetGroupRolesAsync(string groupId, params string[] roleNames);
        Task<TIdentityResult> SetUserGroupsAsync(string userId, params string[] groupIds);
        TIdentityResult SetUserGroups(string userId, params string[] groupIds);
        TIdentityResult RefreshUserGroupRoles(string userId);
        Task<TIdentityResult> RefreshUserGroupRolesAsync(string userId);
        Task<TIdentityResult> DeleteGroupAsync(string groupId);
        TIdentityResult DeleteGroup(string groupId);
        Task<TIdentityResult> UpdateGroupAsync(ApplicationGroup group);
        TIdentityResult UpdateGroup(ApplicationGroup group);
        TIdentityResult ClearUserGroups(string userId);
        Task<TIdentityResult> ClearUserGroupsAsync(string userId);
        IEnumerable<ApplicationGroup> GetUserGroups(string userId);
        Task<IEnumerable<TRole>> GetGroupRolesAsync(string groupId);
        IEnumerable<TRole> GetGroupRoles(string groupId);
        IEnumerable<TUser> GetGroupUsers(string groupId);
        IEnumerable<ApplicationGroupRole> GetUserGroupRoles(string userId);
        ApplicationGroup FindById(string id);
        Task<ApplicationGroup> FindByIdAsync(string id);
    }
}
