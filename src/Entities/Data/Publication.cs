﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Entities.Data
{
    public class Publication
    {
        
        public int Id { get; set; }

        public string PublicationId { get; set; }

        public string Title { get; set; }

        public string Authors { get; set; }

        public string Journal { get; set; }

        public int Year { get; set; }

        public string DOI { get; set; }

        public bool? Accepted { get; set; }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
    
    }
}
