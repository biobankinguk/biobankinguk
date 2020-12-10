﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegacyData
{
    /// <summary>
    /// Material Type terms.
    /// </summary>
    public class MaterialType
    {
        /// <summary>
        /// Internal id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the Material Type (i.e. friendly name)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Many to Many Relationship with MaterialTypeGroup
        /// </summary>
        public ICollection<MaterialTypeGroup> MaterialTypeGroups { get; set; }
    }
}