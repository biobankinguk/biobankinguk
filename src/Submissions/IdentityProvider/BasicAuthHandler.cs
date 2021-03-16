using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Biobanks.IdentityProvider
{
    internal class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly RoleManager<IdentityRole> _roles;

        public BasicAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
            : base(options, logger, encoder, clock)
        {
            _users = userManager;
            _roles = roleManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            //Check we have some credentials string
            string encodedCredentials = null;

            try
            {
                encodedCredentials = AuthenticationHeaderValue.Parse(
                    Request.Headers["Authorization"])
                    .Parameter;
            }
            catch
            {
                // lol we don't care, cos we null check next
            }

            if (string.IsNullOrWhiteSpace(encodedCredentials))
            {
                const string noCredentialsMessage = "No Credentials.";
                Logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            //try and decode the credentials
            string decodedCredentials;

            try
            {
                decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(
                    $"Failed to decode credentials: {encodedCredentials}. {e.Message}");
            }

            //break the decoded string into username/password
            var delimiterIndex = decodedCredentials.IndexOf(':');
            if (delimiterIndex == -1)
            {
                const string missingDelimiterMessage = "Invalid credentials: missing delimiter.";
                Logger.LogInformation(missingDelimiterMessage);
                return AuthenticateResult.Fail(missingDelimiterMessage);
            }
            var username = decodedCredentials.Substring(0, delimiterIndex);
            var password = decodedCredentials.Substring(delimiterIndex + 1);

            // Actually authenticate against Identity
            var user = await _users.FindByNameAsync(username);

            if (await _users.CheckPasswordAsync(user, password))
            {
                var claims = await _users.GetClaimsAsync(user);
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                // TODO if we need more roles in future, make this cleaner?
                if(await _users.IsInRoleAsync(user, CustomRoles.SuperAdmin))
                    claims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", CustomRoles.SuperAdmin));

                //Adding an Authentication type will mark the Identity as authenticated
                var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                Logger.LogInformation($"Credentials validated for {username}");

                return AuthenticateResult.Success(new AuthenticationTicket(
                    claimsPrincipal,
                    Scheme.Name
                ));
            }
            else
            {
                Logger.LogInformation($"Credentials failed validation for {username}");
                return AuthenticateResult.Fail("Invalid credentials.");
            }
        }
    }
}