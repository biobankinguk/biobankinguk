using System.Collections.Generic;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Shared.ReferenceData
{
    /// <summary>
    /// Sniomed Tag entity.
    /// </summary>
    public class SnomedTag : BaseReferenceData
    {
        /// <summary>
        /// Snomed Terms associated with this Tag.
        /// </summary>
        public ICollection<OntologyTerm> OntologyTerms { get; set; }
    }
}
