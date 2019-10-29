using System;
using System.ComponentModel.DataAnnotations;

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
        /// A SNOMED term representing the Diagnosis.
        /// </summary>
        [Required]
        public SnomedTerm DiagnosisCode { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public int DiagnosisCodeOntologyVersionId { get; set; }

        /// <summary>
        /// Ontology version used for diagnosis code.
        /// </summary>
        [Required]
        public OntologyVersion DiagnosisCodeOntologyVersion { get; set; }
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
