using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using Microsoft.VisualBasic.CompilerServices;

namespace Publications
{
     public class GetPublications
      {
        private static readonly HttpClient client = new HttpClient();
        public async Task<List<string>> GetPublicationItems()
        {
            string biobank = "CANDAS";
            
          
            List<string> resultList = new List<string>();
            string nextCursor = "*";
            string currentCursor;
            //API Pagination loop - will stop once current cursor and next cursor are equal (no more records)
            do
            {
                //API endpoint
                string endpoint = $"https://www.ebi.ac.uk/europepmc/webservices/rest/search?query={biobank}&resultType=lite&cursorMark={nextCursor}&pageSize=1000&format=json";
                string response = await client.GetStringAsync(endpoint);
                //Deserialize JSON to object
                RootObject obj = JsonConvert.DeserializeObject<RootObject>(response);

                currentCursor = nextCursor;
                nextCursor = obj.Cursor;
                foreach (var item in obj.Childs.Results)
                    {

                        resultList.Add(item.Id);
                        resultList.Add(item.Title);
                        resultList.Add(item.Authors);
                        resultList.Add(item.Journal);
                        resultList.Add(item.Year.ToString());
                        resultList.Add(item.Doi);
                    
                    }
                
                

            } while (nextCursor!=currentCursor);

          
            return resultList;
        }

    }
}
