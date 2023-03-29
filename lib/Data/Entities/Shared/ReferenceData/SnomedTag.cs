using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;

namespace Biobanks.Entities.Shared.ReferenceData
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