using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class ContentPageModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RouteSlug { get; set; }
        public string Body { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsEnabled { get; set; }
    }
}