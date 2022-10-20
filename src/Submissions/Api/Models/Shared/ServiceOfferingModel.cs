using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class ServiceOfferingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}

