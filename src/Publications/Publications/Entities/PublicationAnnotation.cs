using System;
using System.Collections.Generic;
using System.Text;

namespace Publications.Entities
{
    public class PublicationAnnotation
    {
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }

        public int AnnotationId { get; set; }
        public Annotation Annotation { get; set; }
    }
}
