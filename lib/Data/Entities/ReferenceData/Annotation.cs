using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class Annotation : BaseReferenceData
    {
        public virtual ICollection<Publication> Publications { get; set; }
    }
}
