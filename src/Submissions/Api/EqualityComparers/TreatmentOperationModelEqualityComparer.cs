using System;
using System.Collections.Generic;
using Biobanks.Submissions.Api.Models;

namespace Biobanks.Submissions.Api.EqualityComparers
{
    /// <inheritdoc />
    public class TreatmentOperationModelEqualityComparer : IEqualityComparer<TreatmentOperationModel>
    {
        /// <inheritdoc />
        public bool Equals(TreatmentOperationModel x, TreatmentOperationModel y)
            => x.Treatment.IndividualReferenceId.Equals(y.Treatment.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.Treatment.DateTreated == y.Treatment.DateTreated &&
               x.Treatment.TreatmentCode == y.Treatment.TreatmentCode;

        /// <inheritdoc />
        public int GetHashCode(TreatmentOperationModel obj)
            => (obj.Treatment.IndividualReferenceId +
                obj.Treatment.DateTreated +
                obj.Treatment.TreatmentCode)
                .GetHashCode();
    }
}
