using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Biobanks.Web.Models.Footer
{
    public class FooterModel
    {
        public FooterItemModel Links { get; set; }
        public FooterItemModel Logos { get; set; }
        public string FooterTitle { get; set; }
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