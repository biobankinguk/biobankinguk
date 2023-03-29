using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models
{
    /// <summary>
    /// Overall batch submission model containing all permitted entities and operations.
    /// </summary>
    public class SubmissionModel
    {
        /// <summary>
        /// The Sample entities to process.
        /// </summary>
        public ICollection<SampleOperationModel> Samples { get; set; }

        /// <summary>
        /// The Treatment entities to process.
        /// </summary>
        public ICollection<TreatmentOperationModel> Treatments { get; set; }

        /// <summary>
        /// The Diagnosis entities to process.
        /// </summary>
        public ICollection<DiagnosisOperationModel> Diagnoses { get; set; }
    }
}
