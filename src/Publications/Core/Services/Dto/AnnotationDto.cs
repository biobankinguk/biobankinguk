using Newtonsoft.Json;
using System.Collections.Generic;

namespace Biobanks.Publications.Services.Dto
{
    public class AnnotationResult
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("extId")]
        public string ExtId { get; set; }

        [JsonProperty("annotations")]
        public List<AnnotationDTO> Annotations { get; set; }
    }
    public class AnnotationDTO
    {
        [JsonProperty("exact")]
        public string Exact { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("section")]
        public string Section { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }
    }

    public class Tag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
