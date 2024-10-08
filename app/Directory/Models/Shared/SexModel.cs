﻿using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class SexModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}

