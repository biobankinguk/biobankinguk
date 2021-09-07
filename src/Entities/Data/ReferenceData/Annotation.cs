using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Annotation : ReferenceDataBase
    {
        public virtual ICollection<Publication> Publications { get; set; }
    }
}
