using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Shared
{
    public class RefDataFeedbackModel
    {
        public string Name { get; set; }

        public string RedirectUrl { get; set; }

        public string RefDataType { get; set; }
    }
}