using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace Directory.Auth.Identity
{
    public class DirectoryUserManager : UserManager<DirectoryUser>
    {
        #region ctor

        public DirectoryUserManager(
            IUserStore<DirectoryUser> store,
            Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<DirectoryUser> passwordHasher,
            IEnumerable<IUserValidator<DirectoryUser>> userValidators,
            IEnumerable<IPasswordValidator<DirectoryUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            Microsoft.Extensions.Logging.ILogger<UserManager<DirectoryUser>> logger)
            : base(
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        { }

        #endregion

        /// <summary>
        /// Record the current date and time as the last time the User logged in.
        /// </summary>
        /// <param name="user">The User in question</param>
        public async Task UpdateLastLogin(DirectoryUser user)
        {
            user.LastLogin = DateTimeOffset.UtcNow;
            await Store.UpdateAsync(user, CancellationToken);
        }
    }
}
