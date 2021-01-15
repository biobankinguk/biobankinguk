using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Biobanks.Web.Models.Search;
using Biobanks.Web.Models.ADAC;

namespace Biobanks.Web.Models.Shared
{
    public class TermPageModel
    {
        public IEnumerable<ReadSnomedTermModel> SnomedTermsModel { get; set; }
        public TermpageContentModel TermpageContentModel { get; set; }
    }
}