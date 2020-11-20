using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class AccessConditionsModel
    {
        public ICollection<ReadAccessConditionsModel> AccessConditions;
    }
}