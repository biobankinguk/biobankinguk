using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities.Api
{
    /// <summary>
    /// Common properties for all data owned by organisations.
    /// e.g. Sample, Diagnosis etc.
    /// </summary>
    public class BaseOrganisationDataEntity
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int OrganisationId { get; set; }

        /// <summary>
        /// Creation timestamp of the submission this record is associated with.
        /// </summary>
        public DateTimeOffset SubmissionTimestamp { get; set; }

        /// <summary>
        /// Anonymised identifier for the individual this data is for.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string IndividualReferenceId { get; set; }
    }
}
