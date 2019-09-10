﻿using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Directory.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
            => new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiResource> GetApis()
            => new ApiResource[]
            {
                new ApiResource(ApiResourceKeys.RefData, "UKCRC TDCC Reference Data API"),
                new ApiResource(ApiResourceKeys.Submission, "UKCRC TDCC Submission API")
            };

        public static IEnumerable<Client> GetClients(IConfiguration config)
            => new Client[]
            {
                // TODO: Clients will need to be dynamically stored later
                // as theoretically all installs of vendor apps should
                // register as clients

                // Submission API itself as Client
                new Client
                {
                    ClientId = "submission-app",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(config["ClientSecrets:submission-app"].Sha256())
                    },

                    AllowedScopes = { ApiResourceKeys.RefData }
                },

                // Framework Directory
                
                // Submission Aggregator

                // (Frontend SPA for this app?)

                // Any other first party services?


                // Submission API Clients ...
            };
    }
}
