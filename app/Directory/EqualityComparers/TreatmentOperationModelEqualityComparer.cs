using System;
using System.Collections.Generic;
using Biobanks.Directory.Models.Submissions;

namespace Biobanks.Directory.EqualityComparers
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
