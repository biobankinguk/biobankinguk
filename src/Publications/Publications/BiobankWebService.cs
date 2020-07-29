using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Publications.Services
{
    public class BiobankWebService : IDisposable
    {
        private readonly HttpClient _client;

        public BiobankWebService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://biobankinguk-directory-test.azurewebsites.net/")
            };
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
