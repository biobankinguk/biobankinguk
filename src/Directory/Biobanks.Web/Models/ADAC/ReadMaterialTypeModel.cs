using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadMaterialTypeModel : MaterialTypeModel
    {
        //Sum of all Collections and Capabilities
        public int MaterialDetailCount { get; set; }

    }
}