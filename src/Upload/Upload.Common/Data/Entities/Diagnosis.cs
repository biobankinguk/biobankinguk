using System;
using System.ComponentModel.DataAnnotations;
using Upload.Common.DTO;

namespace Upload.Common.Data.Entities
{
    /// <inheritdoc cref="BaseOrganisationDataEntity" />
    /// <summary>
    /// Diagnosis data for an individual.
    /// </summary>
    public class Diagnosis : BaseOrganisationDataEntity
    {
        /// <summary>
        /// The date of the diagnosis.
        /// </summary>
        public DateTime DateDiagnosed { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public string DiagnosisCodeId { get; set; } = string.Empty;
        /// <summary>
        /// An OMOP term representing the Diagnosis.
        /// </summary>
        [Required]
        public virtual OmopTermDto DiagnosisCode { get; set; } = null!;

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public int DiagnosisCodeOntologyVersionId { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core
    /// </summary>
    public class LiveDiagnosis : Diagnosis { }

    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core
    /// </summary>
    public class StagedDiagnosis : Diagnosis { }
}
