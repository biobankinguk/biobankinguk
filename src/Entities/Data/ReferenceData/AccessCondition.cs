using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AccessCondition : ReferenceDataBase
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
