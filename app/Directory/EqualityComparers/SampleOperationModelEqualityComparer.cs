using System;
using System.Collections.Generic;
using Biobanks.Directory.Models.Submissions;

namespace Biobanks.Directory.EqualityComparers
{
    /// <inheritdoc />
    public class SampleOperationModelEqualityComparer : IEqualityComparer<SampleOperationModel>
    {
        /// <inheritdoc />
        public bool Equals(SampleOperationModel x, SampleOperationModel y)
            => string.Equals(x.Sample.IndividualReferenceId, y.Sample.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(x.Sample.Barcode, y.Sample.Barcode, StringComparison.OrdinalIgnoreCase) && 
               string.Equals(x.Sample.CollectionName, y.Sample.CollectionName, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public int GetHashCode(SampleOperationModel obj)
            => (obj.Sample.IndividualReferenceId +
                obj.Sample.Barcode + 
                obj.Sample.CollectionName)
                .GetHashCode();
    }
}
