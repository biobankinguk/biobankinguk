using System.Collections.Generic;

namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Ontology - for examples, see https://www.ebi.ac.uk/ols/ontologies.
    /// Needs to be reviewed when we move to OMOP
    /// </summary>
    public class Ontology : BaseReferenceDatum
    {
        /// <summary>
        /// Versions available for this ontology.
        /// </summary>
        public virtual List<OntologyVersion> OntologyVersions { get; set; } = new List<OntologyVersion>();
    }
}
