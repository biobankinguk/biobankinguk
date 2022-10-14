using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class RegistrationReasonModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}

