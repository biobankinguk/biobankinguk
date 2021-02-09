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
using Publications.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Publications.Services.Dto;
using Microsoft.Extensions.Logging;

namespace Publications
{
    public class EpmcWebService : IEpmcService
    {

        private readonly HttpClient _client;
        private readonly ILogger<EpmcWebService> _logger;

        public EpmcWebService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<EpmcWebService> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(configuration["EpmcApiUrl"]);
            _logger = logger;
        }

        public async Task<PublicationDto> GetPublicationById(int publicationId)
        {
            return (await PublicationSearch($"{publicationId}")).Publications.FirstOrDefault();
        }

        public async Task<List<AnnotationDto>> GetPublicationAnnotations(string publicationId, string source)
        {
            return (await AnnotationSearch(publicationId, source));
        }

        public async Task<List<PublicationDto>> GetOrganisationPublications(string biobank)
        {
            var publications = new List<PublicationDto>();

            string query = $"ACK_FUND:\"{biobank}\"";
            string nextCursor = "*";
            string currentCursor;

            //API Pagination loop - will stop once current cursor and next cursor are equal (no more records)
            do
            {
                var result = await PublicationSearch(query, nextCursor);

                // Collect publications from paged result
                publications.AddRange(result.Publications);

                // Advance cursor
                currentCursor = nextCursor;
                nextCursor = result.Cursor;
            }
            while (nextCursor != currentCursor);

            return publications;
        }

        private async Task<EpmcSearchResult> PublicationSearch(string query, string cursorMark = "*")
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
            var result = JsonConvert.DeserializeObject<EpmcSearchResult>(response);

            return result;
        }

        private async Task<List<AnnotationDto>> AnnotationSearch(string publicationId, string source)
        {
            var annotations = new List<AnnotationDto>();

            if (string.IsNullOrEmpty(source) || (string.IsNullOrEmpty(publicationId)))
            {
                return annotations;
            }

            // Parse query parameters
            var parameters = new Dictionary<string, string>()
                {
                    { "articleIds", $"{source}:{publicationId}" },
                    { "format", "JSON" }
                };

            string endpoint = QueryHelpers.AddQueryString("annotations_api/annotationsByArticleIds", parameters);
            var result = new List<AnnotationResult>();

            try
            {
                string response = await _client.GetStringAsync(endpoint);

                // Parse JSON result
                result = JsonConvert.DeserializeObject<List<AnnotationResult>>(response);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
            }


            foreach (var annotation in result)
            {
                foreach (var tags in annotation.Annotations)
                {
                    annotations.Add(tags);
                }
            }
            return annotations;
        }

    }
}
