using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Identity.Data.Entities;
using Directory.Data.Caching;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

namespace Biobanks.Identity.Services
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IAuthenticationManager _authenticationManager;

        public ApplicationSignInManager(
            UserManager<ApplicationUser, string> userManager,
            IAuthenticationManager authenticationManager,
            ICacheProvider cacheProvider)
            : base(userManager, authenticationManager)
        {
            _cacheProvider = cacheProvider;
            _authenticationManager = authenticationManager;
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, CookieAuthenticationDefaults.AuthenticationType);
        }

        public async Task<SignInStatus> LocalSignInAsync(string userName, string password, bool isPersistent)
        {
            if (UserManager == null) return SignInStatus.Failure;

            // Getting the user by the user name.
            var user = await UserManager.FindByNameAsync(userName);
            
            // If no user then fail, but track attempts for lock out, as if the user exists
            if (user == null)
            {
                //Check the memory cache to see if this username is locked out
                int attempts;
                try
                {
                    attempts = _cacheProvider.Retrieve<int>(userName);
                }
                catch (KeyNotFoundException)
                {
                    attempts = 0;
                }

                var lockedOut = attempts >= UserManager.MaxFailedAccessAttemptsBeforeLockout;

                //Behave accordingly
                if (lockedOut) return SignInStatus.LockedOut;

                //Update the number of attempts in the cache
                _cacheProvider.Store<int>(userName, ++attempts, UserManager.DefaultAccountLockoutTimeSpan);

                return SignInStatus.Failure;
                
            }

            if (!user.EmailConfirmed) return SignInStatus.RequiresVerification;

            return await PasswordSignInAsync(userName, password, isPersistent, true); //also handles account lockouts for us :)
        }

        public async Task RefreshSignInAsync(ApplicationUser user)
        {
            // Check if current session is persistent
            var auth = await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);
            var isPersistent = auth.Properties.IsPersistent;

            // Sign-Out / Sign-In
            _authenticationManager.SignOut();
            await SignInAsync(user, isPersistent, true);
        }
    }
}
