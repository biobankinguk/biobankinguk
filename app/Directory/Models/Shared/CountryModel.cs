using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class CountryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

