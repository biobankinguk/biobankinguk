﻿using Newtonsoft.Json.Linq;

namespace Biobanks.Submissions.Api.Models
{
    /// <summary>
    /// ViewModel representing an individual error message and the identifying properties of the record it relates to.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Unique identifier of the error.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Error message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Unique identifiers of the record to which the error relates.
        /// </summary>
        public JObject RecordIdentifiers { get; set; }
    }
}