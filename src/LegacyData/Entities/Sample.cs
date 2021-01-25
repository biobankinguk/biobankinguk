using Biobanks.Entities.Api;
using Biobanks.Entities.Api.Contracts;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacyData.Entities
{
    /// <inheritdoc cref="BaseOrganisationDataEntity" />
    /// <summary>
    /// Sample data for an individual.
    /// </summary>
    public class Sample : BaseOrganisationDataEntity, ISubmissionTimestamped
    {
        /// <summary>
        /// An organisation specific
        /// (and unique within organisation) sample identifier.
        /// </summary>
        [Required]
        public string Barcode { get; set; }

        /// <summary>
        /// Optional Year of birth of the individual.
        /// At least one of Age or YearOfBirth are required.
        /// </summary>
        public int? YearOfBirth { get; set; }

        /// <summary>
        /// Optional Age of the individual when the sample was donated.
        /// At least one of Age or YearOfBirth are required.
        /// </summary>
        public int? AgeAtDonation { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int MaterialTypeId { get; set; }
        /// <summary>
        /// The type of sample material.
        /// </summary>
        [Required]
        public MaterialType MaterialType { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? StorageTemperatureId { get; set; }
        /// <summary>
        /// The storage temperature of this sample.
        /// </summary>
        public StorageTemperature StorageTemperature { get; set; }

        /// <summary>
        /// The date the sample was "created" e.g. extracted.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public string ExtractionSiteId { get; set; }
        /// <summary>
        /// A SNOMED term representing the site of extraction of this tissue sample.
        /// Tissue Sample only.
        /// </summary>
        public SnomedTerm ExtractionSite { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? ExtractionSiteOntologyVersionId { get; set; }

        /// <summary>
        /// Ontology version used for sample extraction site.
        /// </summary>
        public OntologyVersion ExtractionSiteOntologyVersion { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public string ExtractionProcedureId { get; set; }
        /// <summary>
        /// A SNOMED term for the sample extraction procedure.
        /// Extracted Samples only.
        /// </summary>
        public SnomedTerm ExtractionProcedure { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public string SampleContentId { get; set; }
        /// <summary>
        /// A SNOMED term describing the sample content.
        /// Exracted Samples only.
        /// </summary>
        public SnomedTerm SampleContent { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? SampleContentMethodId { get; set; }
        /// <summary>
        /// Method of acquisition for the sample content.
        /// Extracted Samples only.
        /// </summary>
        public SampleContentMethod SampleContentMethod { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? SexId { get; set; }

        /// <summary>
        /// Sex of the individual.
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// Name of the collection to associate the sample with.
        /// </summary>
        public string CollectionName { get;set; }
    }

    /// <inheritdoc />
    public class Sample<TSampleAvailability> : Sample
        where TSampleAvailability : class
    {
        /// <summary>
        /// Join entities for Availibility constraints which apply to this sample.
        /// </summary>
        public ICollection<TSampleAvailability> SampleAvailabilities { get; set; }
    }

    /// <summary>
    /// Subclass for EF Core
    /// </summary>
    public class LiveSample : Sample { }

    /// <summary>
    /// Subclass for EF Core
    /// </summary>
    public class StagedSample : Sample { }
}
