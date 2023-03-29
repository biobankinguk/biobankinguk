using System.Collections.Generic;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class AssociatedDataType : BaseReferenceData
    {
        public string Message { get; set; }

        public int AssociatedDataTypeGroupId { get; set; }

        public virtual AssociatedDataTypeGroup AssociatedDataTypeGroup { get; set; }

        public List<OntologyTerm> OntologyTerms { get; set; }

    }
}
