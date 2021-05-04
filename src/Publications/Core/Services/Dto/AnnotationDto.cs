using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Biobanks.Publications.Core.Services.Dto
{
    public class AnnotationResult
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("extId")]
        public string ExtId { get; set; }

        [JsonPropertyName("annotations")]
        public List<AnnotationDTO> Annotations { get; set; }
    }
    public class AnnotationDTO
    {
        [JsonPropertyName("exact")]
        public string Exact { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
