using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Shared
{
    public class ServiceOfferingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
