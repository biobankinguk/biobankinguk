using System.ComponentModel.DataAnnotations;
using Biobanks.Common.Data.Entities.ReferenceData;

namespace LegacyData.Entities.JoinEntities
{
    /// <summary>
    /// Many to many join table for Material Types and Material Type Groups
    /// </summary>
    public class MaterialTypeMaterialTypeGroup
    {
        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int MaterialTypeId { get; set; }
        /// <summary>
        /// The Material Type in the relationship.
        /// </summary>
        [Required]
        public MaterialType MaterialType { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int MaterialTypeGroupId { get; set; }
        /// <summary>
        /// The Material Type Group in the relationship.
        /// </summary>
        [Required]
        public MaterialTypeGroup MaterialTypeGroup { get; set; }
    }
}