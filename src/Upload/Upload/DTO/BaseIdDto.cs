using System;
using System.ComponentModel.DataAnnotations;

namespace Upload.DTO
{
    public class BaseIdDto
    {
        /// <summary>
        /// Foreign key column.
        /// </summary>
        [Required]
        public int OrganisationId { get; set; }

        [Required]
        public DateTimeOffset SubmissionTimestamp { get; set; }

        /// <summary>
        /// A non-identifiable reference number for the individual
        /// </summary>
        [Required]
        public string IndividualReferenceId { get; set; } = string.Empty;
    }
}
