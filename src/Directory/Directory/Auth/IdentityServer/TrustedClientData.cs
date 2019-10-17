using System.Collections.Generic;
using Common.Constants;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Directory.Auth.IdentityServer
{
    /// <summary>
    /// Purely a configuration class.
    /// These Clients are seeded into the config db, as they are trusted.
    /// Third party Clients should be added to the database, not here.
    /// </summary>
    public static class TrustedClientData
    {
        /// <summary>
        /// All the preconfigured Clients
        /// </summary>
        /// <param name="config">To allow reading environment config values</param>
        public static IEnumerable<Client> List(IConfiguration config)
            => new Client[]
            {
                DirectoryWebApp(config[$"TrustedClients:{TrustedClientIds.DirectoryWebApp}:origin"]),
                UploadApi(config[$"TrustedClients:{TrustedClientIds.UploadApi}:secret"]),

                // MVC PoC Clients // TODO: REMOVE when ready
                MvcPoc("Hybrid",
                    config["TrustedClients:mvc-poc:origin"],
                    config[$"TrustedClients:{TrustedClientIds.UploadApi}:secret"]), // HACK: Don't re-use secrets for reals
                MvcPoc("PKCE",
                    config["TrustedClients:mvc-poc:origin"],
                    config[$"TrustedClients:{TrustedClientIds.UploadApi}:secret"]) // HACK: Don't re-use secrets for reals

                // TODO: Framework Directory

                // TODO: Submission Aggregator

                // TODO: Any other first party services?
            };

        /// <summary>
        /// The Directory Frontend WebApp
        /// </summary>
        /// <param name="origin">The origin of the app urls</param>
        private static Client DirectoryWebApp(string origin)
            => new Client
            {
                ClientId = TrustedClientIds.DirectoryWebApp,
                ClientName = "UKCRC Tissue Directory",
                AllowedGrantTypes = GrantTypes.Code,
                RequireConsent = false,
                RequirePkce = true,
                RequireClientSecret = false,

                RedirectUris = { $"{origin}/auth/signin-oidc" },
                PostLogoutRedirectUris = { $"{origin}/auth/signout-callback-oidc" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    ApiResourceKeys.RefData
                }
            };

        /// <summary>
        /// The Upload API
        /// </summary>
        /// <param name="plainSecret">A plain value for a ClientSecret which will be hashed by this method.</param>
        private static Client UploadApi(string plainSecret)
            => new Client
            {
                ClientId = TrustedClientIds.UploadApi,
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets = { new Secret(plainSecret.Sha256()) },

                AllowedScopes = { ApiResourceKeys.RefData }
            };

        // TODO: PoC only - REMOVE when ready
        /// <summary>
        /// <para>Proof of Concept MVC (or other Server Side Web App) Client.</para>
        /// 
        /// <para>
        /// This provides both PKCE and Hybrid Flow versions.
        /// PKCE should be used anywhere it's supported.
        /// </para>
        /// 
        /// <para>We may not know until we try if ASP.NET 4.x (e.g. the Framework Directory) supports PKCE.</para>
        /// 
        /// <para>
        /// Bear in mind MVC is a server-side app and can therefore also use Client Credentials
        /// for client-level API access tokens if it wants,
        /// though if a user is logged in their access token may be easer, if appropriate.
        /// </para>
        /// </summary>
        /// <param name="type">"PKCE" or "Hybrid" to determine the flow configuration.</param>
        /// <param name="origin">The origin of the app urls</param>
        /// <param name="plainSecret">A plain value for a ClientSecret which will be hashed by this method.</param>
        private static Client MvcPoc(string type, string origin, string plainSecret)
            => new Client
            {
                ClientId = $"mvc-poc-{type.ToLower()}",
                ClientName = $"MVC Proof of Concept ({type} flow)",
                AllowedGrantTypes = type == "Hybrid"
                    ? GrantTypes.HybridAndClientCredentials
                    : GrantTypes.CodeAndClientCredentials,
                RequireConsent = false,
                RequirePkce = type == "PKCE",

                ClientSecrets = { new Secret(plainSecret.Sha256()) },

                RedirectUris = { $"{origin}/signin-oidc" },
                PostLogoutRedirectUris = { $"{origin}/signout-callback-oidc" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    ApiResourceKeys.RefData
                },
                AllowOfflineAccess = true
            };
    }
}
