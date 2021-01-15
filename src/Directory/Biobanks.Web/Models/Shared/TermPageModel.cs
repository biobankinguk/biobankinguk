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
        public List<SnomedTermModel> DiagnosesModel { get; set; }
        public TermpageContentModel TermpageContentModel { get; set; }
    }
}