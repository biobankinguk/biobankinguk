using System;
using System.ComponentModel.DataAnnotations;

namespace Upload.Common.Data.Entities
{
    /// <inheritdoc cref="BaseOrganisationDataEntity" />
    /// <summary>
    /// Treatment data for an individual.
    /// </summary>
    public class Treatment : BaseOrganisationDataEntity
    {
        /// <summary>
        /// The date of treatment.
        /// </summary>
        public DateTime DateTreated { get; set; }

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        [Required]
        public string TreatmentCodeId { get; set; } = string.Empty;

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public int? TreatmentLocationId { get; set; }

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        [Required]
        public int TreatmentCodeOntologyVersionId { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core
    /// </summary>
    public class LiveTreatment : Treatment { }

    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core
    /// </summary>
    public class StagedTreatment : Treatment { }
}
