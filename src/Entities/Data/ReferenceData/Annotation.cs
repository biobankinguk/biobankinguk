using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Annotation
    {
        public int Id { get; set; }

        public string AnnotationId { get; set; }
        public string Name { get; set; }

        public string Uri { get; set; }

        public virtual ICollection<Publication> Publications { get; set; }
    }
}
