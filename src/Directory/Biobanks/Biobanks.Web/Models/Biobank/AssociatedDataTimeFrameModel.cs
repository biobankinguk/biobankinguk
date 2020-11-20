using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Biobanks.Web.Models.Biobank
{
    public class AssociatedDataTimeFrameModel
    {
        public int ProvisionTimeId { get; set; }
        public string ProvisionTimeDescription { get; set; }

        public string ProvisionTimeValue { get; set; }

    }
}