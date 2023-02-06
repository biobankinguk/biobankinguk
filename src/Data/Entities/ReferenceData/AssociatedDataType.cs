using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataType : BaseReferenceData
    {
        public string Message { get; set; }

        public int AssociatedDataTypeGroupId { get; set; }

        public virtual AssociatedDataTypeGroup AssociatedDataTypeGroup { get; set; }

        public List<OntologyTerm> OntologyTerms { get; set; }

    }
}
