using Directory.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Publications.Entities
{
    public class Publication
    { 
        [Key]
        public int Id { get; set; }

        [Required]
        public string PublicationId { get; set; }

        [Required]
        public int OrganisationId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Authors { get; set; }

        public string Journal { get; set; }

        public int Year { get; set; }

        public string DOI { get; set; }

        public bool? Accepted { get; set; }

        public string Source { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual ICollection<PublicationAnnotation> PublicationAnnotations { get; set; }
    }
}
