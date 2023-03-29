using System.Collections.Generic;
using Newtonsoft.Json;

namespace Biobanks.Directory.Models.Header
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
