using Newtonsoft.Json;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Header
{
    public class HeaderItemModel
    {
        public string Title { get; set; }

        public string Src { get; set; }

        public string Url { get; set; }

        [JsonProperty("classes")]
        public IEnumerable<string> StyleClasses;
    }
}