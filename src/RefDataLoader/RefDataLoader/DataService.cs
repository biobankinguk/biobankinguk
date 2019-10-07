using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RefDataLoader
{
    public class DataService : IDataService
    {
        public static HttpClient Client;

        public void SeedData()
        {

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
