using System;
using System.Collections.Generic;
using Biobanks.Data.Entities.Api;

namespace Biobanks.Directory.EqualityComparers
{
    /// <inheritdoc />
    public class SampleEqualityComparer : EqualityComparer<Sample>
    {
        /// <inheritdoc />
        public override bool Equals(Sample x, Sample y)
            => x.OrganisationId == y.OrganisationId &&
               x.IndividualReferenceId.Equals(y.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               x.Barcode.Equals(y.Barcode, StringComparison.OrdinalIgnoreCase) &&
               x.CollectionName.Equals(y.CollectionName, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public override int GetHashCode(Sample obj)
            => (obj.OrganisationId +
                obj.IndividualReferenceId +
                obj.Barcode +
                obj.CollectionName).GetHashCode();
    }
}
