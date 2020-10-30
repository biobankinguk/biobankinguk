using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Biobank
{
    public class AssociatedDataGroupModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public List<AssociatedDataModel> Types { get; set; }

    }
}