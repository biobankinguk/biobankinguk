using System;
using System.Collections.Generic;
using Biobanks.Data.Entities.Api;

namespace Biobanks.Directory.EqualityComparers
{
    /// <inheritdoc />
    public class TreatmentEqualityComparer : IEqualityComparer<Treatment>
    {
        /// <inheritdoc />
        public bool Equals(Treatment x, Treatment y)
            => x.OrganisationId == y.OrganisationId &&
               x.IndividualReferenceId.Equals(y.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.DateTreated == y.DateTreated &&
               x.TreatmentCode == y.TreatmentCode;

        /// <inheritdoc />
        public int GetHashCode(Treatment obj)
            => (obj.OrganisationId +
                obj.IndividualReferenceId +
                obj.DateTreated +
                obj.TreatmentCode)
                .GetHashCode();
    }
}
