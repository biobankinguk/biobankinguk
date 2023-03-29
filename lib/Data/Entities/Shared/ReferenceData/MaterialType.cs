using System.Collections.Generic;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Shared.ReferenceData
{
    /// <summary>
    /// Material Type terms.
    /// </summary>
    public class MaterialType : BaseReferenceData
    {
        /// <summary>
        /// Many to Many Relationship with MaterialTypeGroup
        /// </summary>
        public ICollection<MaterialTypeGroup> MaterialTypeGroups { get; set; }

        /// <summary>
        /// Many to Many Relationship with OntologyTerm
        /// </summary>
        public ICollection<OntologyTerm> ExtractionProcedures { get; set; }
    }
}
