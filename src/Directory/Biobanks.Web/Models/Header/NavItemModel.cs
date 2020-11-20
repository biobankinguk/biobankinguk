using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Header
{
    public class NavItemModel
    {
        public string Title { get; set; }

        public string Action { get; set; }

        public string Url { get; set; }

        [JsonProperty("classes")]
        public IEnumerable<string> StyleClasses;

        [JsonProperty("child_items")]
        public IEnumerable<NavItemModel> Children;
    }
}