using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Analytics.Services.Contracts;

namespace Analytics.Services
{
    public class BiobankWebService : IBiobankWebService
    {
        private readonly HttpClient _client;

        public BiobankWebService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(configuration["DirectoryUrl"]);

        }

        public async Task<List<string>> GetOrganisationNames()
        {
            var response = await _client.GetStringAsync("/api/biobanks");
            var result = JsonConvert.DeserializeObject<List<string>>(response);

            return result;
        }

        public async Task<List<string>> GetOrganisationExternalIds()
        {
            var response = await _client.GetStringAsync("/api/biobankids");
            var result = JsonConvert.DeserializeObject<List<string>>(response);


            return result;
        }
    }
}
