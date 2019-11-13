using System.ComponentModel.DataAnnotations;
using Upload.Common.Types;

namespace Upload.DTO
{
    /// <inheritdoc />
    public class TreatmentSubmissionDto : TreatmentIdDto
    {
        /// <summary>
        /// The anatomical location where the treatment was performed.
        /// </summary>
        public string TreatmentLocation { get; set; } = string.Empty;

        /// <summary>
        /// The ontology for the given treatment location.
        /// </summary>
        /// <seealso cref="TreatmentLocation"/>
        /// <seealso cref="TreatmentCodeOntologyVersion"/>
        [Required]
        public string TreatmentCodeOntology { get; set; } = string.Empty;

        /// <summary>
        /// The version of the ontology for the given treatment location.
        /// </summary>
        /// <seealso cref="TreatmentLocation"/>
        /// <seealso cref="TreatmentCodeOntology"/>
        [Required]
        public string TreatmentCodeOntologyVersion { get; set; } = string.Empty;

        /// <summary>
        /// Ontology field to which the code relates.
        /// TODO: This may be slightly confusing as possible values are code and value, but the field is still called code.
        /// </summary>
        public OntologyField TreatmentCodeOntologyField { get; set; } = OntologyField.Code;
    }
}
