using Biobanks.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Utilities.IdentityModel;

namespace Biobanks.Submissions.Api.Auth.Basic
{
    internal class BasicAuthHandler : AuthenticationHandler<BasicAuthSchemeOptions>
    {
        private readonly ApplicationDbContext _db;

        public BasicAuthHandler(
            IOptionsMonitor<BasicAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationDbContext db)
            : base(options, logger, encoder, clock)
        {
            _db = db;
        }

        private (string username, string password) ParseBasicAuthHeader(string authorizationHeader)
        {
            AuthenticationHeaderValue.TryParse(authorizationHeader, out var header);

            if (string.IsNullOrWhiteSpace(header?.Parameter))
            {
                const string noCredentialsMessage = "No Credentials.";
                Logger.LogError(noCredentialsMessage);
                throw new BasicAuthParsingException(noCredentialsMessage);
            }

            List<string> credentialsParts;

            try
            {
                // decode the header parameter
                credentialsParts = Encoding.UTF8.GetString(
                        Convert.FromBase64String(header.Parameter))
                    .Split(":", 2) // split at the first colon only
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Failed to decode credentials: {header.Parameter}.");

                throw new BasicAuthParsingException(
                    $"Failed to decode credentials: {header.Parameter}.",
                    e);
            }

            if (credentialsParts.Count < 2)
            {
                const string invalidCredentials = "Invalid credentials: missing delimiter.";
                Logger.LogError(invalidCredentials);
                throw new BasicAuthParsingException(invalidCredentials);
            }

            return (credentialsParts[0], credentialsParts[1]);
        }

        private async Task<ClaimsPrincipal> Authenticate(string clientId, string clientSecret)
        {
            var client = await _db.ApiClients.AsNoTracking()
                .Include(x => x.Organisations)
                .SingleOrDefaultAsync(x => x.ClientId == clientId);

            if (client is null) return null;

            // Secret validation is straightforward as we expect secrets to be suitably complex
            // This is supported by our secret generation library code
            // and is suitably secure as per: https://github.com/IdentityServer/IdentityServer4/issues/284
            if (clientSecret.Sha256() != client.ClientSecretHash)
                return null;

            // Successful Credentials Validation - create a ClaimsPrincipal

            // Populate Claims
            List<Claim> claims = new();
            if (client.Organisations.Any())
            {
                claims.AddRange(client.Organisations
                    .Select(o => new Claim(
                        CustomClaimTypes.BiobankId,
                        o.OrganisationId.ToString()))); // TODO: is this the correct ID that clients will be submitting?
            }
            else
            {
                // TODO: Fix SuperAdmin to use formal persisted scopes
                // instead of assuming no org == superuser ;)
                claims.Add(new Claim(ClaimTypes.Role, CustomRoles.SuperUser));
            }
            claims.Add(new Claim(CustomClaimTypes.ClientId, clientId));


            // Create the Identity and Principal
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            return new ClaimsPrincipal(identity);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var (clientId, clientSecret) = ParseBasicAuthHeader(Request.Headers["Authorization"]);

                var claimsPrincipal = await Authenticate(clientId, clientSecret);

                if (claimsPrincipal is not null)
                {
                    Logger.LogInformation($"Credentials validated for Client: {clientId}");

                    return AuthenticateResult.Success(new AuthenticationTicket(
                        claimsPrincipal,
                        Scheme.Name
                    ));
                }
                else
                {
                    Logger.LogInformation($"Credentials failed validation for Client: {clientId}");
                    return AuthenticateResult.Fail("Invalid credentials.");
                }

            }
            catch (BasicAuthParsingException e)
            {
                return AuthenticateResult.Fail(e.Message);
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"{Scheme.Name} realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
