using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Biobank
{
    public class AcceptanceModel
    {
        public ICollection<NetworkAcceptanceModel> NetworkRequests { get; set; }
    }
}