﻿using System;
using System.ComponentModel.DataAnnotations;
using Entities.Api.Contracts;
using Entities.Api.ReferenceData;
using Entities.Shared.ReferenceData;

namespace Entities.Api
{
    /// <inheritdoc cref="BaseOrganisationDataEntity" />
    /// <summary>
    /// Treatment data for an individual.
    /// </summary>
    public class Treatment : BaseOrganisationDataEntity, ISubmissionTimestamped
    {
        /// <summary>
        /// The date of treatment.
        /// </summary>
        public DateTime DateTreated { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public string TreatmentCodeId { get; set; }
        /// <summary>
        /// A SNOMED term representing the Treatment.
        /// </summary>
        [Required]
        public SnomedTerm TreatmentCode { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? TreatmentLocationId { get; set; }
        /// <summary>
        /// Optional Treatment Location.
        /// </summary>
        public TreatmentLocation TreatmentLocation { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public int TreatmentCodeOntologyVersionId { get; set; }

        /// <summary>
        /// Ontology version used for treatment code.
        /// </summary>
        [Required]
        public OntologyVersion TreatmentCodeOntologyVersion { get; set; }
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
