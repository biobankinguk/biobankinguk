using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Publications.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Publications.Services
{
    public class BiobankWebService : IBiobankService, IDisposable
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

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
