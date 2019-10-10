using Common.DTO;
using Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RefDataLoader
{
    public class DataService : IDataService
    {
        public static HttpClient Client;
        private readonly ApiSettings _config;

        public DataService(IOptions<ApiSettings> options)
        {
            _config = options.Value;
        }


        public void SeedData()
        {
            PrepareHttpClient();
            // Read Data into DTOs for each refData type
            PrepData("AccessCondition.json");

            // POST in the correct order (e.g. groups first) to the API


        }



        private void PrepareHttpClient()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(_config.Baseuri)
            };
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task SubmitData(IList<string> data)
        {
            //We post ontologies individually.

            var timer = Stopwatch.StartNew();
            for (var i = 0; i < data.Count; i++)
            {
                var materialTypeGroup = data[i];

                var result = await SendJsonAsync("MaterialTypeGroup", JsonConvert.SerializeObject(materialTypeGroup));

                Console.WriteLine(
                    $"MaterialTypeGroup {materialTypeGroup}" +
                    $" ({i} / {data.Count}):" +
                    $" Post {(result ? "successful" : "failed")}");
            }

            timer.Stop();
            Console.WriteLine($"MaterialTypeGroup Posts took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {timer.Elapsed}");
        }

        private List<RefDataBaseDto> PrepData(string datafile)
        {
            var timer = Stopwatch.StartNew();

            var o = JsonConvert.DeserializeObject<List<RefDataBaseDto>>(datafile);

            //abuse config builders to load the data ;)
            var data = new ConfigurationBuilder()
                .AddJsonFile(datafile, optional: false)
                .Build();

            return o;
        }

        private static async Task<bool> SendJsonAsync(string endpoint, string data)
        {
            var r = await Client.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    //Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"]) }, //todo replace with token given by provider
                    RequestUri = new Uri(Client.BaseAddress, endpoint),
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                });

            return r.IsSuccessStatusCode;
        }

        private static async Task<string> GetJsonAsync(string endpoint)
        {
            var r = (await Client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                //Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"]) }, //todo replace with token given by provider
                RequestUri = new Uri(Client.BaseAddress, endpoint)
            })).EnsureSuccessStatusCode();

            return await r.Content.ReadAsStringAsync();
        }
    }

    public interface IDataService
    {
        void SeedData();
    }
}
