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

        public IEnumerable<CountriesDTO> ListCountries()
        {
            var response = _client.GetStringAsync(_uri).Result;
            var result = JsonConvert.DeserializeObject<IEnumerable<CountriesDTO>>(response);

            return result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
