using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Common.Data.Entities.JoinEntities;

namespace Biobanks.Common.Data.Entities.ReferenceData
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
        /// Join entities for Material Type Groups that this type is a member of.
        /// </summary>
        public ICollection<MaterialTypeGroup> MaterialTypeGroups { get; set; }
    }
}