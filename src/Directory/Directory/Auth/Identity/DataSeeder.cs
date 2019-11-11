using System;
using System.Threading.Tasks;
using Common.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Directory.Auth.Identity
{
    public static class DataSeeder
    {
        public static async Task Seed(DirectoryUserManager users, IPasswordHasher<DirectoryUser> passwords, IConfiguration config)
        {
            // Seed an initial super user to use for setup
            if (await users.FindByNameAsync("superadmin@localhost") is null)
            {
                // check an actual password has been configured
                var pwd = config["SuperAdminSeedPassword"];
                if (string.IsNullOrEmpty(pwd))
                {
                    throw new ApplicationException(@"
A non-empty password must be configured for seeding the inital SuperAdmin User.
Please set SuperAdminSeedPassword in a user secrets file,
or the environment variable ASPNETCORE_SuperAdminSeedPassword");
                }

                var user = new DirectoryUser
                {
                    UserName = "superadmin@localhost",
                    Email = "superadmin@localhost",
                    Name = "Super Admin",
                    EmailConfirmed = true
                };

                user.PasswordHash = passwords.HashPassword(user, pwd);

                await users.CreateAsync(user);
            }
        }
    }
}
