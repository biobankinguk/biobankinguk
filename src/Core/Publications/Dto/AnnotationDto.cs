using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Biobanks.Publications.Dto
{
    public class AnnotationResultDto
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("extId")]
        public string ExtId { get; set; }

        [JsonPropertyName("annotations")]
        public List<AnnotationDto> Annotations { get; set; }
    }

    public class AnnotationDto
    {
        [JsonPropertyName("exact")]
        public string Exact { get; set; }

        [JsonPropertyName("tags")]
        public List<TagDto> Tags { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }
    }

    public class TagDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
