using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadCollectionStatusModel : Shared.CollectionStatusModel
    {
        public int CollectionCount { get; set; }
    }
}