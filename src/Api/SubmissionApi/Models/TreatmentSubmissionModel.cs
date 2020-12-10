﻿using System.ComponentModel.DataAnnotations;
using Biobanks.Common.Models;
using Biobanks.Common.Types;

namespace Biobanks.SubmissionApi.Models
{
    /// <inheritdoc />
    public class TreatmentSubmissionModel : TreatmentIdModel
    {
        /// <summary>
        /// The anatomical location where the treatment was performed.
        /// </summary>
        public string TreatmentLocation { get; set; }

        /// <summary>
        /// The ontology for the given treatment location.
        /// </summary>
        /// <seealso cref="TreatmentLocation"/>
        /// <seealso cref="TreatmentCodeOntologyVersion"/>
        [Required]
        public string TreatmentCodeOntology { get; set; }

        /// <summary>
        /// The version of the ontology for the given treatment location.
        /// </summary>
        /// <seealso cref="TreatmentLocation"/>
        /// <seealso cref="TreatmentCodeOntology"/>
        [Required]
        public string TreatmentCodeOntologyVersion { get; set; }

        /// <summary>
        /// Ontology field to which the code relates.
        /// TODO: This may be slightly confusing as possible values are code and value, but the field is still called code.
        /// </summary>
        public OntologyField TreatmentCodeOntologyField { get; set; } = OntologyField.Code;
    }
}
