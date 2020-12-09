﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Common.Data.Entities.ReferenceData
{
    /// <summary>
    /// Sniomed Tag entity.
    /// </summary>
    public class SnomedTag
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the term (i.e. friendly name)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Snomed Terms associated with this Tag.
        /// </summary>
        public ICollection<SnomedTerm> SnomedTerms { get; set; }
    }
}