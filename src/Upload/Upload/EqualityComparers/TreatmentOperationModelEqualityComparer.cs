using System;
using System.Collections.Generic;
using Upload.DTOs;

namespace Upload.EqualityComparers
{
    /// <inheritdoc />
    public class TreatmentOperationModelEqualityComparer : IEqualityComparer<TreatmentOperationDto>
    {
        /// <inheritdoc />
        public bool Equals(TreatmentOperationDto x, TreatmentOperationDto y)
            => x.Treatment.IndividualReferenceId.Equals(y.Treatment.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.Treatment.DateTreated == y.Treatment.DateTreated &&
               x.Treatment.TreatmentCode == y.Treatment.TreatmentCode;

        /// <inheritdoc />
        public int GetHashCode(TreatmentOperationDto obj)
            => (obj.Treatment.IndividualReferenceId +
                obj.Treatment.DateTreated +
                obj.Treatment.TreatmentCode)
                .GetHashCode();
    }
}
