using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Publications
{
    public class EPMCSearchResult
    {
        [JsonProperty("nextCursorMark")]
        public string Cursor { get; set; }

        [JsonProperty("resultList")]
        public Results Results { get; set; }

        public List<Publication> Publications
        {
            get { return Results.Publications; }
        }
    }

    public class Results
    {
        [JsonProperty("result")]
        public List<Publication> Publications { get; set; }
    }

    public class Publication
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("authorString")]
        public string Authors { get; set; }

        [JsonProperty("journalTitle")]
        public string Journal { get; set; }

        [JsonProperty("pubYear")]
        public int Year { get; set; }

        [JsonProperty("doi")]
        public string Doi { get; set; }

    }
}
