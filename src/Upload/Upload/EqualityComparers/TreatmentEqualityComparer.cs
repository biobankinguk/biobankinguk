using System;
using System.Collections.Generic;
using Common.Data.Upload;

namespace Biobanks.SubmissionApi.EqualityComparers
{
    /// <inheritdoc />
    public class TreatmentEqualityComparer : IEqualityComparer<Treatment>
    {
        /// <inheritdoc />
        public bool Equals(Treatment x, Treatment y)
            => x.OrganisationId == y.OrganisationId &&
               x.IndividualReferenceId.Equals(y.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.DateTreated == y.DateTreated &&
               x.TreatmentCodeId == y.TreatmentCodeId;

        /// <inheritdoc />
        public int GetHashCode(Treatment obj)
            => (obj.OrganisationId +
                obj.IndividualReferenceId +
                obj.DateTreated +
                obj.TreatmentCodeId)
                .GetHashCode();
    }
}
