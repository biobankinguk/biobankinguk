using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Biobanks.Publications.Core.Services.Dto
{
    public class EpmcSearchResult
    {
        [JsonPropertyName("nextCursorMark")]
        public string Cursor { get; set; }

        [JsonPropertyName("resultList")]
        public Results Results { get; set; }

        public List<PublicationDto> Publications => Results.Publications;
    }

    public class Results
    {
        [JsonPropertyName("result")]
        public List<PublicationDto> Publications { get; set; }
    }

    public class PublicationDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("authorString")]
        public string Authors { get; set; }

        [JsonPropertyName("journalTitle")]
        public string Journal { get; set; }

        [JsonPropertyName("pubYear")]
        public int Year { get; set; }

        public string Doi { get; set; }
        
        public string Source { get; set; }

    }
}
