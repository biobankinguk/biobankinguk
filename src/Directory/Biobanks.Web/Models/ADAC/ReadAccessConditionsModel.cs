using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadAccessConditionsModel : AccessConditionModel
    {
        //Sum of all Collections and Capabilities
        public int AccessConditionCount { get; set; }

    }
}