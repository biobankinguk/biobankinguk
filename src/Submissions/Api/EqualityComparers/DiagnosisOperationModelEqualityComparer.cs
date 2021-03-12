using System;
using System.Collections.Generic;
using Biobanks.Submissions.Api.Models;

namespace Biobanks.Submissions.Api.EqualityComparers
{
    /// <inheritdoc />
    public class DiagnosisOperationModelEqualityComparer : IEqualityComparer<DiagnosisOperationModel>
    {
        /// <inheritdoc />
        public bool Equals(DiagnosisOperationModel x, DiagnosisOperationModel y)
            => x.Diagnosis.IndividualReferenceId.Equals(y.Diagnosis.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.Diagnosis.DateDiagnosed == y.Diagnosis.DateDiagnosed &&
               x.Diagnosis.DiagnosisCode == y.Diagnosis.DiagnosisCode;

        /// <inheritdoc />
        public int GetHashCode(DiagnosisOperationModel obj)
            => (obj.Diagnosis.IndividualReferenceId +
                obj.Diagnosis.DateDiagnosed +
                obj.Diagnosis.DiagnosisCode)
                .GetHashCode();
    }
}
