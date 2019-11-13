using System.Collections.Generic;

namespace Upload.DTO
{
    /// <summary>
    /// Overall batch submission dto containing all permitted entities and operations.
    /// </summary>
    public class SubmissionDto
    {
        /// <summary>
        /// The Sample entities to process.
        /// </summary>
        public ICollection<SampleOperationDto> Samples { get; set; } = new List<SampleOperationDto>();

        /// <summary>
        /// The Treatment entities to process.
        /// </summary>
        public ICollection<TreatmentOperationDto> Treatments { get; set; } = new List<TreatmentOperationDto>();

        /// <summary>
        /// The Diagnosis entities to process.
        /// </summary>
        public ICollection<DiagnosisOperationDto> Diagnoses { get; set; } = new List<DiagnosisOperationDto>();
    }
}
