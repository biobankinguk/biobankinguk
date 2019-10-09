using System.Collections.Generic;
using Common.Constants;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace Directory.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
            => new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static List<TestUser> GetUsers()
            => new List<TestUser>
            {
                new TestUser { SubjectId = "1", Username = "jon@jon.jon", Password="test"},
                new TestUser { SubjectId = "2", Username = "bob@bob.bob", Password="test"}
            };

        public static IEnumerable<ApiResource> GetApis()
            => new ApiResource[]
            {
                new ApiResource(ApiResourceKeys.RefData, "UKCRC TDCC Reference Data API"),
                new ApiResource(ApiResourceKeys.Upload, "UKCRC TDCC Upload API")
            };

        public static IEnumerable<Client> GetClients(IConfiguration config)
            => new Client[]
            {
                // TODO: Some Clients will need to be dynamically stored later
                // as theoretically all installs of vendor apps should
                // register as clients

                // Client-side WebApp for the (new) Directory
                new Client
                {
                    ClientId = TrustedClientIds.DirectoryWebApp,
                    ClientName = "UKCRC Tissue Directory",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "https://localhost:5001/auth/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5001/auth/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        ApiResourceKeys.RefData
                    }
                },

                // Submission API itself as Client
                new Client
                {
                    ClientId = TrustedClientIds.UploadApi,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(config[$"ClientSecrets:{TrustedClientIds.UploadApi}"].Sha256())
                    },

                    AllowedScopes = { ApiResourceKeys.RefData }
                },

                // MVC PoC client // TODO: Remove when ready
                new Client
                {
                    ClientId = "mvc-poc",
                    ClientName = "MVC Proof of Concept",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequireConsent = false,

                    ClientSecrets =
                    {
                        // Don't reuse secrets for realsies!
                        new Secret(config[$"ClientSecrets:{TrustedClientIds.UploadApi}"].Sha256())
                    },

                    RedirectUris = { "https://localhost:5201/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5201/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        ApiResourceKeys.RefData
                    },
                    AllowOfflineAccess = true
                },

                // Framework Directory
                
                // Submission Aggregator

                // Any other first party services?


                // Submission API Clients ...
            };
    }
}
