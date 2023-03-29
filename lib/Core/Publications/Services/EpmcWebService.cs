using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Flurl;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Publications.Dto;


namespace Biobanks.Publications.Services
{
    public class EpmcWebService : IEpmcService
    {

        private readonly HttpClient _client;
        private readonly ILogger<EpmcWebService> _logger;

        public EpmcWebService(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, 
            ILogger<EpmcWebService> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(configuration["Publications:EpmcApiUrl"]);
            _logger = logger;
        }

        public async Task<PublicationDto> GetPublicationById(int publicationId)
            => (await PublicationSearch($"{publicationId}")).Publications.FirstOrDefault();

        public async Task<List<AnnotationDto>> GetPublicationAnnotations(string publicationId, string source)
            => (await AnnotationSearch(publicationId, source));

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

            // Parse JSON results
            try
            {
                return JsonSerializer.Deserialize<EpmcSearchResult>(response);
            }
            catch (JsonException e)
            {
                throw new Exception($"JSON Parse Exception for { query } at cursor mark { cursorMark }", e);
            }
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


            // Filter by type of Annotation
            var types = new List<string>()
            {
                "Diseases",
                "Organ Tissue",
                "Phenotype",
                "Sample-Material",
                "Body-Site"
            };

            var url = new Url("annotations_api/annotationsByArticleIds");
            url.SetQueryParams(parameters).SetQueryParam("type", types);
            var result = new List<AnnotationResultDto>();

            try 
            {
                var response = await _client.GetStringAsync(url);

                // Parse JSON result
                result = JsonSerializer.Deserialize<List<AnnotationResultDto>>(response);

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
