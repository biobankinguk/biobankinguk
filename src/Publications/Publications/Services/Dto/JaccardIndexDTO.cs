using System;
using System.Collections.Generic;
using System.Text;
using Publications.Entities;

namespace Publications.Services.Dto
{
    public class JaccardIndexDTO
    {
        public int OrganisationId { get; set; }
        public double JaccardIndex { get; set; }
        public string CommonAnnotations { get; set; }

        public Publication Publication { get; set; }
    }
}
