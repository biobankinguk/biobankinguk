using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Models.ADAC
{
    public class ReadAccessConditionsModel : AccessConditionModel
    {
        //Sum of all Collections and Capabilities
        /// <summary> 
        /// Access Condition Count
        ///</summary>
        ///<example>0</example>
        public int AccessConditionCount { get; set; }


    }
}
