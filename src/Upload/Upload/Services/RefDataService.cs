using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Constants;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace Upload.Services
{
    // TODO: This is a dummy service to test authorised calls to the dummy refdata API
    // It should not be long lived

    public class RefDataService
    {
        private readonly IConfiguration _config;

        public RefDataService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IList<string>> GetUserClaimsFromRefDataApi()
        {
            // 1. Get OIDC Discovery Document from server (do this once, early?)
            var client = new HttpClient { BaseAddress = new Uri(_config["JwtBearer:Authority"]) };
            var disco = await client.GetDiscoveryDocumentAsync(_config["JwtBearer:Authority"]);
            if (disco.IsError) throw new Exception(
                $"Discovery Document could not be found on OIDC Server at {_config["JwtBearer:Authority"]}");

            // 2. Get JWT from Token Endpoint with valid credentials
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = TrustedClientIds.Upload,
                ClientSecret = _config[$"ClientSecrets:{TrustedClientIds.Upload}"],

                Scope = ApiResourceKeys.RefData
            });
            if (tokenResponse.IsError) throw new Exception("Unable to get OIDC Token");

            // 3. Make an API Request with the token
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("identity");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Request failed with Error: {response.StatusCode}");

            return JsonSerializer.Deserialize<List<string>>(await response.Content.ReadAsStringAsync());
        }
    }
}
