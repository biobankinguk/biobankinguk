﻿using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class CollectionStatusModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
        public int SortOrder { get; set; }
    }
}
