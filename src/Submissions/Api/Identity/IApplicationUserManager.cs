using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Identity
{
    public interface IApplicationUserManager<TUser, TKey, TIdentityResult>
    {
        Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType);
        Task<TUser> FindByNameAsync(string userName);
        TUser FindById(TKey userId);
        Task<TUser> FindByIdAsync(TKey userId);
        IList<string> GetRoles(TKey userId);
        Task<IList<string>> GetRolesAsync(TKey userId);
        TIdentityResult RemoveFromRoles(TKey userId, params string[] roles);
        Task<TIdentityResult> RemoveFromRolesAsync(TKey userId, params string[] roles);
        TIdentityResult AddToRoles(TKey userId, params string[] roles);
        Task<TIdentityResult> AddToRolesAsync(TKey userId, params string[] roles);
        IQueryable<TUser> Users { get; }
        Task<TIdentityResult> CreateAsync(TUser user);
        Task<TIdentityResult> CreateAsync(TUser user, string password);
        Task<TIdentityResult> UpdateAsync(TUser user);
        Task<TIdentityResult> DeleteAsync(TUser user);

        Task<string> GenerateEmailConfirmationTokenAsync(TKey userId);
        Task<TIdentityResult> ConfirmEmailAsync(TKey userId, string token);
        Task<TUser> FindByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(TKey userId);
        Task<TIdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);
        Task UpdateLastLogin(string userId);
    }
}
