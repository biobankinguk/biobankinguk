using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.DataLoader
{
    public partial class Program
    {
        public static async Task<bool> SendJsonAsync(string endpoint, string data)
        {
            var r = await Client.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Headers = {Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"])},
                    RequestUri = new Uri(Client.BaseAddress, endpoint),
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                });

            return r.IsSuccessStatusCode;
        }

        public static async Task<string> GetJsonAsync(string endpoint)
        {
            var r = (await Client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Headers = {Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"])},
                RequestUri = new Uri(Client.BaseAddress, endpoint)
            })).EnsureSuccessStatusCode();

            return await r.Content.ReadAsStringAsync();
        }
    }
}