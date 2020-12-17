using System.Collections.Generic;
using LegacyData.Entities.JoinEntities;
using System.ComponentModel.DataAnnotations;

namespace LegacyData.Entities
{
    /// <summary>
    /// This is an internal entity for artificially grouping material types.
    /// It is used for conditional validation against certain material types.
    /// It can be used for any future purposes.
    /// </summary>
    public class MaterialTypeGroup
    {
        /// <summary>
        /// Internal id for the group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of the group.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Join entities for MaterialTypes in the group
        /// </summary>
        public ICollection<MaterialTypeMaterialTypeGroup> MaterialTypes { get; set; }
    }
}