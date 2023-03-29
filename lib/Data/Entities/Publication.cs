using System;
using System.Collections.Generic;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class Publication
    {
        
        public int Id { get; set; }

        public string PublicationId { get; set; }

        public string Title { get; set; }

        public string Authors { get; set; }

        public string Journal { get; set; }

        public int? Year { get; set; }

        public string DOI { get; set; }

        public bool? Accepted { get; set; }

        public string Source { get; set; }

        public DateTime? AnnotationsSynced { get; set; }
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public virtual ICollection<Annotation> Annotations { get; set; }
    
    }
}
