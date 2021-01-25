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
        private Uri _uri;
        private IConfiguration _configuration;
        public CountriesWebService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient();
            _configuration = configuration;
            _uri = new Uri(_configuration["CountriesApiKey"]);

        }

        public async Task<List<CountriesDTO>> GetCountries()
        {
            List<CountriesDTO> result = await Search();
            return result; 
        }
        private async Task<List<CountriesDTO>> Search()
        {

            string response = await _client.GetStringAsync(_uri);
            List<CountriesDTO> result = JsonConvert.DeserializeObject<List<CountriesDTO>>(response);
            return result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

    }
}
