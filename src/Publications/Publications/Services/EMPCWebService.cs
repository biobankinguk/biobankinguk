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
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

namespace Publications
{
     public class EMPCWebService : IDisposable
     {

        private readonly HttpClient _client;
        
        public EMPCWebService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://www.ebi.ac.uk/europepmc/")
            };
        }

        public async Task<Publication> GetPublicationById(int publicationId)
        {
            return (await Search($"{publicationId}")).Publications.FirstOrDefault();
        }

        public async Task<List<Publication>> GetOrganisationPublications(string biobank)
        {
            List<Publication> publications = new List<Publication>();
            
            string query = $"ACK_FUND:\"{biobank}\"";
            string nextCursor = "*";
            string currentCursor;

            //API Pagination loop - will stop once current cursor and next cursor are equal (no more records)
            do
            {
                EPMCSearchResult result = await Search(query, nextCursor);

                // Collect publications from paged result
                publications.AddRange(result.Publications);
                
                // Advance cursor
                currentCursor = nextCursor;
                nextCursor = result.Cursor;
            } 
            while (nextCursor != currentCursor);

            return publications;
        }

        private async Task<EPMCSearchResult> Search(string query, string cursorMark="*")
        {
            // Parse query parameters
            var parameters = new Dictionary<string, string>()
                {
                    { "query", query },
                    { "cursorMark", cursorMark },
                    { "resultType", "lite" },
                    { "pageSize", "1000" },
                    { "format", "json" }
                };

            string endpoint = QueryHelpers.AddQueryString("webservices/rest/search", parameters);
            string response = await _client.GetStringAsync(endpoint);
            
            // Parse JSON result
            EPMCSearchResult result = JsonConvert.DeserializeObject<EPMCSearchResult>(response);

            return result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
