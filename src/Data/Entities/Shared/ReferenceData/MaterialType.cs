using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;

namespace Biobanks.Entities.Shared.ReferenceData
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