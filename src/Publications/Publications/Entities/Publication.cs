using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Publications.Entities
{
    public class Publication
    { 
        [Key]
        public int InternalId { get; set; }

        public int PublicationId { get; set; }

        public string Organisation { get; set; }

        public string Title { get; set; }

        public string Authors { get; set; }

        public string Journal { get; set; }

        public int Year { get; set; }

        public string DOI { get; set; }
    }
}
