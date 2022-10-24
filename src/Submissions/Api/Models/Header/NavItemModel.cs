using Newtonsoft.Json;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Header
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