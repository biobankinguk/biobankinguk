using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AccessCondition : BaseReferenceData
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
