using System;
using System.Collections.Generic;
using System.Text;

namespace Publications.Entities
{
    public class PublicationAnnotation
    {
        public int PublicationsId { get; set; }
        public Publication Publication { get; set; }

        public int AnnotationsId { get; set; }
        public Annotation Annotation { get; set; }
    }
}
