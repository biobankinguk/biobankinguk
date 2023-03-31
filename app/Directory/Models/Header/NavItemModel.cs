using System.Collections.Generic;
using Newtonsoft.Json;

namespace Biobanks.Directory.Models.Header
{
    public class NavItemModel
    {
        public string Title { get; set; }

        public string Action { get; set; }

        public string Url { get; set; }

        public bool Toolbar { get; set; }

        [JsonProperty("classes")]
        public IEnumerable<string> StyleClasses;

        [JsonProperty("child_items")]
        public IEnumerable<NavItemModel> Children;
    }
}
