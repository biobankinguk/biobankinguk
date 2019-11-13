using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Upload.Common.Data.Entities
{
    /// <inheritdoc cref="BaseOrganisationDataEntity" />
    /// <summary>
    /// Sample data for an individual.
    /// </summary>
    public class Sample : BaseOrganisationDataEntity
    {
        /// <summary>
        /// An organisation specific
        /// (and unique within organisation) sample identifier.
        /// </summary>
        [Required]
        public string Barcode { get; set; } = string.Empty;

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
        /// Weak Foreign key column.
        /// </summary>
        public int MaterialTypeId { get; set; }

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public int? StorageTemperatureId { get; set; }

        /// <summary>
        /// The date the sample was "created" e.g. extracted.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public string ExtractionSiteId { get; set; } = string.Empty;

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public int? ExtractionSiteOntologyVersionId { get; set; }

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public string ExtractionProcedureId { get; set; } = string.Empty;

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public string SampleContentId { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? SampleContentMethodId { get; set; }

        /// <summary>
        /// Weak Foreign key column.
        /// </summary>
        public int? SexId { get; set; }

        /// <summary>
        /// Name of the collection to associate the sample with.
        /// </summary>
        public string CollectionName { get; set; } = string.Empty;
    }

    /// <inheritdoc />
    public class Sample<TSampleAvailability> : Sample where TSampleAvailability : class
    {
        /// <summary>
        /// Join entities for Availibility constraints which apply to this sample.
        /// </summary>
        public List<TSampleAvailability> SampleAvailabilities { get; set; } = new List<TSampleAvailability>();
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
