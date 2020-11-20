using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Directory.Identity.Data;
using Directory.Identity.Contracts;
using Directory.Identity.Data.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Directory.Identity.Services
{
    public class ApplicationUserManager : UserManager<ApplicationUser, string>, IApplicationUserManager<ApplicationUser, string, IdentityResult>
    {
        private readonly int _passwordHistoryLimit;

        public ApplicationUserManager(IUserStore<ApplicationUser, string> store, IDataProtectionProvider dataProtectionProvider)
            : base(store)
        {

            // Configure validation logic for usernames
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Password policy restrictions
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8, // University policy requires min 7; OWASP 8
                RequireDigit = true, // University policy, OWASP
                RequireLowercase = true, // University policy, OWASP
                RequireUppercase = true, // University policy, OWASP
                RequireNonLetterOrDigit = true, // University policy, OWASP
            };
            _passwordHistoryLimit = 8; // University policy - can't reuse last 8 passwords!

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true; //University Policy
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5); //University Policy specifies no time...
            MaxFailedAccessAttemptsBeforeLockout = 3; //University Policy


            // Two Factor?


            //Configure Data Protection Provider for Tokens (reset password, account confirmation etc...)
            if (dataProtectionProvider != null)
            {
                UserTokenProvider =
                    new NoPlussesTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromDays(5) //sadly, these can't differ for password / confirm, would need a separate user manager
                    };
            }
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<UserStoreDbContext>()),
                options.DataProtectionProvider);

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8, // University policy requires min 7; OWASP 8
                RequireDigit = true, // University policy, OWASP
                RequireLowercase = true, // University policy, OWASP
                RequireUppercase = true, // University policy, OWASP
                RequireNonLetterOrDigit = true, // University policy, OWASP
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new NoPlussesTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public ApplicationUser FindById(string userId)
        {
            // because IApplicationUserManager exposes this method, the extension method needs to be explicitly called
            // in order to provide it to interface clients
            return UserManagerExtensions.FindById(this, userId);
        }

        public IList<string> GetRoles(string userId)
        {
            return UserManagerExtensions.GetRoles(this, userId);
        }

        public IdentityResult RemoveFromRoles(string userId, params string[] roles)
        {
            return UserManagerExtensions.RemoveFromRoles(this, userId, roles);
        }

        public IdentityResult AddToRoles(string userId, params string[] roles)
        {
            return UserManagerExtensions.AddToRoles(this, userId, roles);
        }

        public override async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            //First check against historical passwords
            if (await IsHistoricalPassword(userId, newPassword))
            {
                return await Task.FromResult(IdentityResult.Failed("Your new password cannot be the same as a previous one."));
            }

            //try to change password as normal
            var result = await base.ChangePasswordAsync(userId, currentPassword, newPassword);

            if (result.Succeeded)
            {
                //change succeeded? add the hash to the history table now
                var store = Store as ApplicationUserStore;
                await store.AddToHistoricalPasswordsAsync(
                    await FindByIdAsync(userId),
                    PasswordHasher.HashPassword(newPassword));
            }

            return result;
        }

        public async Task UpdateLastLogin(string userId)
        {
            var store = Store as ApplicationUserStore;
            await store.UpdateLastLogin(await FindByIdAsync(userId));
        }

        private async Task<bool> IsHistoricalPassword(string userId, string newPassword)
        {
            var user = await FindByIdAsync(userId);

            if (user.HistoricalPasswords
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => x.PasswordHash)
                    .Take(_passwordHistoryLimit)
                    .Any(x => PasswordHasher.VerifyHashedPassword(x, newPassword) != PasswordVerificationResult.Failed))
            {
                return true;
            }

            return false;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            //First check against historical passwords
            if (await IsHistoricalPassword(userId, newPassword))
            {
                return await Task.FromResult(IdentityResult.Failed("Your new password cannot be the same as a previous one."));
            }

            //try to change password as normal
            var result = await base.ResetPasswordAsync(userId, token, newPassword);

            if (result.Succeeded)
            {
                //change succeeded? add the hash to the history table now
                var store = Store as ApplicationUserStore;
                await store.AddToHistoricalPasswordsAsync(
                    await FindByIdAsync(userId),
                    PasswordHasher.HashPassword(newPassword));
            }

            return result;
        }
    }
}
