using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;

namespace Biobanks.Entities.Api.ReferenceData
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