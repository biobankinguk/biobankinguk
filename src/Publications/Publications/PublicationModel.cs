using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Publications
{

    public class RootObject
    {
        [JsonProperty("resultList")]
        public Child Childs { get; set; }

        [JsonProperty("nextCursorMark")] //To get next page mark
        public string Cursor { get; set; }

    }


    public class Child
    {
        [JsonProperty("result")]
        public List<Result> Results { get; set; }

    }
    public class Result
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
