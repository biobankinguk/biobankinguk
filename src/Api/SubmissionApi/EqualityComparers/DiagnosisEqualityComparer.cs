using System;
using System.Collections.Generic;
using Biobanks.Common.Data.Entities;

namespace Biobanks.SubmissionApi.EqualityComparers
{
    /// <inheritdoc />
    public class DiagnosisEqualityComparer : EqualityComparer<Diagnosis>
    {
        /// <inheritdoc />
        public override bool Equals(Diagnosis x, Diagnosis y)
            => x.OrganisationId == y.OrganisationId &&
               x.IndividualReferenceId.Equals(y.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.DateDiagnosed == y.DateDiagnosed &&
               x.DiagnosisCodeId == y.DiagnosisCodeId;

        /// <inheritdoc />
        public override int GetHashCode(Diagnosis obj)
            => (obj.OrganisationId +
                obj.IndividualReferenceId +
                obj.DateDiagnosed +
                obj.DiagnosisCodeId)
                .GetHashCode();
    }
}
