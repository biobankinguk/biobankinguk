using System.Collections.Generic;
using Newtonsoft.Json;

namespace Biobanks.Directory.Models.Footer
{
    public class FooterModel
    {
        public FooterItemModel Links { get; set; }
        public FooterItemModel Logos { get; set; }
        public string Title { get; set; }
        public string CopyrightText { get; set; }
    }

    public class FooterItemModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Src { get; set; }

        [JsonProperty("child_items")]
        public IEnumerable<FooterItemModel> Children { get; set; }
    }
}
