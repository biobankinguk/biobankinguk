using System;
using System.ComponentModel.DataAnnotations;
using Upload.Common.Models;

namespace Common.Data.Upload
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
        public string DiagnosisCodeId { get; set; }
        /// <summary>
        /// An OMOP term representing the Diagnosis.
        /// </summary>
        [Required]
        public virtual OmopTerm DiagnosisCode { get; set; }

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
