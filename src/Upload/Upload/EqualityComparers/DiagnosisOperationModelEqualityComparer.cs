using System;
using System.Collections.Generic;
using Upload.DTOs;

namespace Upload.EqualityComparers
{
    /// <inheritdoc />
    public class DiagnosisOperationModelEqualityComparer : IEqualityComparer<DiagnosisOperationDto>
    {
        /// <inheritdoc />
        public bool Equals(DiagnosisOperationDto x, DiagnosisOperationDto y)
        {
            if(x.Diagnosis is null && y.Diagnosis is null)
            {
                return true;
            }
            else if(x.Diagnosis is null || y.Diagnosis is null)
            {
                return false;
            }
            else
            {
               return x.Diagnosis.IndividualReferenceId.Equals(y.Diagnosis.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.Diagnosis.DateDiagnosed == y.Diagnosis.DateDiagnosed &&
               x.Diagnosis.DiagnosisCode == y.Diagnosis.DiagnosisCode;
            }
        }
            

        /// <inheritdoc />
        public int GetHashCode(DiagnosisOperationDto obj)
        {
            if (obj.Diagnosis is null)
                return string.Empty.GetHashCode();
            else
            {
                return (obj.Diagnosis.IndividualReferenceId +
                obj.Diagnosis.DateDiagnosed +
                obj.Diagnosis.DiagnosisCode)
                .GetHashCode();
            }
        }
            
    }
}
