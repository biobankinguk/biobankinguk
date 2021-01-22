using System;
using System.Threading.Tasks;
using Biobanks.Identity.Data;
using Biobanks.Identity.Data.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Biobanks.Identity.Services
{
    public class ApplicationUserStore
        : UserStore<ApplicationUser, ApplicationRole, string,
            ApplicationUserLogin, ApplicationUserRole,
            ApplicationUserClaim>, IUserStore<ApplicationUser, string>,
               IDisposable
    {
        public ApplicationUserStore(UserStoreDbContext context)
            : base(context)
        {
            DisposeContext = false;
        }

        public override async Task CreateAsync(ApplicationUser user)
        {
            await base.CreateAsync(user);

            await AddToHistoricalPasswordsAsync(user, user.PasswordHash);
        }

        public async Task AddToHistoricalPasswordsAsync(ApplicationUser user, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash)) return;

            user.HistoricalPasswords.Add(new HistoricalPassword
            {
                UserId = user.Id,
                PasswordHash = passwordHash
            });

            await UpdateAsync(user);
        }

        public async Task UpdateLastLogin(ApplicationUser user)
        {
            user.LastLogin = DateTime.Now;

            await UpdateAsync(user);
        }
    }
}
