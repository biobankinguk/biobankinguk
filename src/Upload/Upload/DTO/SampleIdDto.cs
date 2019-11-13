using System;
using System.ComponentModel.DataAnnotations;

namespace Upload.DTO
{
    public class SampleIdDto
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        /// <summary>
        /// A non-identifiable reference number for the individual
        /// </summary>
        [Required]
        public string IndividualReferenceId { get; set; } = string.Empty;

        /// <summary>
        /// The unique identifier for the sample
        /// </summary>
        [Required]
        public string Barcode { get; set; } = string.Empty;

        /// <summary>
        /// Name of the collection to add the sample to.
        /// </summary>
        [StringLength(250)]
        public string CollectionName { get; set; } = string.Empty;
    }
}
