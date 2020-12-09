﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Common.Models
{
    public class TreatmentIdModel
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        /// <summary>
        /// A non-identifiable reference number for the individual
        /// </summary>
        [Required]
        public string IndividualReferenceId { get; set; }

        public DateTime DateTreated { get; set; }

        [Required]
        public string TreatmentCode { get; set; }
    }
}
