using System;
using System.Collections.Generic;
using System.Text;

namespace Publications.Services.Dto
{
    public class AnnotationQueryDTO
    {
        public int? OrganisationId { get; set; }

        public string Annotation { get; set; }
        public List<string> Annotations { get; set; }
    }
}
