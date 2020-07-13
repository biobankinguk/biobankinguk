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
            //API endpoint
            string response = await client.GetStringAsync("https://www.ebi.ac.uk/europepmc/webservices/rest/search?query=RIO&resultType=lite&cursorMark=*&pageSize=1000&format=json");
            //Deserialize JSON to object
            RootObject obj = JsonConvert.DeserializeObject<RootObject>(response);

            return obj;
        }

    }
}
