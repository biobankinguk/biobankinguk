using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Biobanks.DataSeed.Dto;
using Newtonsoft.Json;

namespace Biobanks.DataSeed.Services
{
    public class CountriesWebService : IDisposable
    {
        private readonly HttpClient _client;
        private readonly Uri _uri;
        
        public CountriesWebService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient();
            _uri = new Uri(configuration["CountriesApiKey"]);
        }

        public async Task<IEnumerable<CountriesDTO>> ListCountriesAsync()
        {
            var response = await _client.GetStringAsync(_uri);
            var result = JsonConvert.DeserializeObject<IEnumerable<CountriesDTO>>(response);

            return result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
