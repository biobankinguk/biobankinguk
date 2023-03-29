using System.Collections.Generic;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Api.ReferenceData
{
    /// <summary>
    /// Ontology - for examples, see https://www.ebi.ac.uk/ols/ontologies.
    /// </summary>
    public class Ontology : BaseReferenceData
    {
        /// <summary>
        /// Versions available for this ontology.
        /// </summary>
        public ICollection<OntologyVersion> OntologyVersions { get; set; }
    }
}
