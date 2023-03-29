using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Annotation : BaseReferenceData
    {
        public virtual ICollection<Publication> Publications { get; set; }
    }
}
