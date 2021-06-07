using Newtonsoft.Json;

namespace Biobanks.Web.Models.Shared
{
    public class PublicationSearchModel
    {
        [JsonProperty("id")]
        public string PublicationId { get; set; }

        public string Title { get; set; }

        [JsonProperty("authorString")]
        public string Authors { get; set; }

        [JsonProperty("journalTitle")]
        public string Journal { get; set; }

        [JsonProperty("pubYear")]
        public int Year { get; set; }

        public string DOI { get; set; }

    }
}
