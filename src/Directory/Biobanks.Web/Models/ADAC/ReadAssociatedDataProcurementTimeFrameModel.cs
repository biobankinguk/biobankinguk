using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadAssociatedDataProcurementTimeFrameModel : Shared.AssociatedDataProcurementTimeFrameModel
    {
        public int CollectionCapabilityCount { get; set; }
    }
}