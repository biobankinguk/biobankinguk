using Newtonsoft.Json;
using System.Collections.Generic;

namespace Biobanks.Web.ApiClientModels
{
    public class WordPressNavItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        [JsonProperty("classes")]
        public IEnumerable<string> StyleClasses;

        [JsonProperty("child_items")]
        public IEnumerable<WordPressNavItem> Children;
    }
}
