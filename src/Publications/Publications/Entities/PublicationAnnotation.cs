using System;
using System.Collections.Generic;
using System.Text;

namespace Publications.Entities
{
    public class PublicationAnnotation
    {
        public int Publication_Id { get; set; }
        public Publication Publication { get; set; }

        public int Annotation_Id { get; set; }
        public Annotation Annotation { get; set; }
    }
}
