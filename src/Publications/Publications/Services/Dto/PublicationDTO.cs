using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Publications
{
    public class EpmcSearchResult
    {
        [JsonProperty("nextCursorMark")]
        public string Cursor { get; set; }

        [JsonProperty("resultList")]
        public Results Results { get; set; }

        public List<PublicationDto> Publications => Results.Publications;
    }

    public class Results
    {
        [JsonProperty("result")]
        public List<PublicationDto> Publications { get; set; }
    }

    public class PublicationDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonProperty("authorString")]
        public string Authors { get; set; }

        [JsonProperty("journalTitle")]
        public string Journal { get; set; }

        [JsonProperty("pubYear")]
        public int Year { get; set; }

        public string Doi { get; set; }

    }
}
