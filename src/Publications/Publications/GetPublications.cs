using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;


namespace Publications
{
     public class GetPublications
      {
        private static readonly HttpClient client = new HttpClient();
        public async Task<RootObject> GetPublicationItems()
        {
            string biobank = "AstraZeneca";
            //API endpoint
            string endpoint = $"https://www.ebi.ac.uk/europepmc/webservices/rest/search?query={biobank}&resultType=lite&cursorMark=AoIIQFqUuSg0MDkzOTczMA==&pageSize=1000&format=json";
            string response = await client.GetStringAsync(endpoint);
            //Deserialize JSON to object
            RootObject obj = JsonConvert.DeserializeObject<RootObject>(response);

            return obj;
        }

    }
}
