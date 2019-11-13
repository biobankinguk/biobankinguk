using System;
using System.Collections.Generic;
using Upload.DTO;

namespace Upload.EqualityComparers
{
    /// <inheritdoc />
    public class SampleOperationModelEqualityComparer : IEqualityComparer<SampleOperationDto>
    {
        /// <inheritdoc />
        public bool Equals(SampleOperationDto x, SampleOperationDto y)
            => string.Equals(x.Sample.IndividualReferenceId, y.Sample.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(x.Sample.Barcode, y.Sample.Barcode, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(x.Sample.CollectionName, y.Sample.CollectionName, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public int GetHashCode(SampleOperationDto obj)
            => (obj.Sample.IndividualReferenceId +
                obj.Sample.Barcode +
                obj.Sample.CollectionName)
                .GetHashCode();
    }
}
