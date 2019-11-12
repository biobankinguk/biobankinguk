using System;
using System.ComponentModel.DataAnnotations;

namespace Upload.DTOs
{
    public class TreatmentIdDto
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        /// <summary>
        /// A non-identifiable reference number for the individual
        /// </summary>
        [Required]
        public string IndividualReferenceId { get; set; } = string.Empty;

        public DateTime DateTreated { get; set; }

        [Required]
        public string TreatmentCode { get; set; } = string.Empty;
    }
}
