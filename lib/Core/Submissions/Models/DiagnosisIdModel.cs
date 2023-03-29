using System;
using System.ComponentModel.DataAnnotations;
using Biobanks.Submissions.Types;

namespace Biobanks.Submissions.Models
{
    public class DiagnosisIdModel : BaseIdModel
    {
        /// <summary>
        /// The date of the diagnosis.
        /// </summary>
        [Required]
        public DateTime? DateDiagnosed { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public string DiagnosisCode { get; set; }

        /// <summary>
        /// Ontology name for DiagnosisCode
        /// </summary>
        [Required]
        public string DiagnosisCodeOntology { get; set; }

        /// <summary>
        /// Ontology version for DiagnosisCode
        /// </summary>
        [Required]
        public string DiagnosisCodeOntologyVersion { get; set; }

        /// <summary>
        /// Ontology field to which the code relates.
        /// TODO: This may be slightly confusing as possible values are code and value, but the field is still called code.
        /// </summary>
        public OntologyField DiagnosisCodeOntologyField { get; set; } = OntologyField.Code;
    }
}
