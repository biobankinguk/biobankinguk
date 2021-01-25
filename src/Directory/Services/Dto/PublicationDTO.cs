using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Dto
{
    public class PublicationDTO
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
