﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class MaterialTypeGroupModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int MaterialTypeCount { get; set; }

        public IEnumerable<string> MaterialTypes { get; set; }
    }
}

