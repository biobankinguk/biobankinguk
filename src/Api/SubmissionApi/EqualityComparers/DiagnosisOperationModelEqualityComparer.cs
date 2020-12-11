﻿using System;
using System.Collections.Generic;
using Biobanks.SubmissionApi.Models;

namespace Biobanks.SubmissionApi.EqualityComparers
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
        {
            var a = (obj.Diagnosis.IndividualReferenceId +
                obj.Diagnosis.DateDiagnosed +
                obj.Diagnosis.DiagnosisCode)
                .GetHashCode();

            return a;
        }
    }
}
