using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Biobanks.IdentityProvider
{
    public partial class Startup
    {
        private void SeedUsers(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task.Run(async () =>
            {
                // first make sure superadmin role exists
                if (!await roleManager.RoleExistsAsync(CustomRoles.SuperAdmin))
                {
                    await roleManager.CreateAsync(new IdentityRole(CustomRoles.SuperAdmin));
                }

                var existingUsernames = userManager.Users.Select(x => x.UserName).ToList();

                foreach (var userItem in Configuration.GetSection("BiobankUsers").GetChildren())
                {
                    var parsedUser = ParseSeedUserString(userItem.Value);

                    if (!existingUsernames.Contains(parsedUser.username))
                    {
                        var user = await userManager.FindByNameAsync(parsedUser.username);

                        if (user == null)
                        {
                            user = new IdentityUser(parsedUser.username);
                            await userManager.CreateAsync(user, parsedUser.password);
                        }

                        foreach (var biobankId in parsedUser.biobankIds)
                        {
                            await userManager.AddClaimAsync(user, new Claim(
                                CustomClaimTypes.BiobankId, biobankId));
                        }
                    }
                }

                foreach (var userItem in Configuration.GetSection("SuperAdminUsers").GetChildren())
                {
                    var parsedUser = ParseSeedUserString(userItem.Value);

                    if (!existingUsernames.Contains(parsedUser.username))
                    {
                        var user = await userManager.FindByNameAsync(parsedUser.username);

                        if (user == null)
                        {
                            user = new IdentityUser(parsedUser.username);
                            await userManager.CreateAsync(user, parsedUser.password);
                        }

                        await userManager.AddToRoleAsync(user, CustomRoles.SuperAdmin);
                    }
                }
            }).Wait();
        }

        private static (string username, string password, string[] biobankIds)
            ParseSeedUserString(string seedUserString)
        {
            if (!(seedUserString.Contains(':') && seedUserString.Contains('@')))
                throw new ArgumentException(
                    $"{nameof(seedUserString)} is not formatted correctly: 'user:pass@id'");

            var iFirstColon = seedUserString.IndexOf(':');
            var iLastAt = seedUserString.LastIndexOf('@');

            var username = seedUserString.Substring(0, iFirstColon);
            var password = seedUserString.Substring(iFirstColon + 1, iLastAt - (iFirstColon + 1));
            var biobankIds = seedUserString.Substring(iLastAt + 1).Split(',');

            return (username, password, biobankIds);
        }
    }
}