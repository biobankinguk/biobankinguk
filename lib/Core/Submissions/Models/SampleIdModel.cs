using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Models
{
    public class SampleIdModel
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        /// <summary>
        /// A non-identifiable reference number for the individual
        /// </summary>
        [Required]
        public string IndividualReferenceId { get; set; }

        /// <summary>
        /// The unique identifier for the sample
        /// </summary>
        [Required]
        public string Barcode { get; set; }

        /// <summary>
        /// Name of the collection to add the sample to.
        /// </summary>
        [StringLength(250)]
        public string CollectionName { get; set; }
    }
}
