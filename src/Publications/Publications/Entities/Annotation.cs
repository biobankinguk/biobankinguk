using System;
using System.Collections.Generic;
using System.Text;

namespace Publications.Entities
{
    public class Annotation
    {
        public int Id { get; set; }

        public string AnnotationId { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public virtual ICollection<PublicationAnnotation> PublicationAnnotations { get; set; }
    }
}
