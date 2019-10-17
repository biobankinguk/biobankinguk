namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Represents a term from a given ontology list (currently only Snomed).
    /// </summary>
    public class OntologyTerm
    {
        /// <summary>
        /// This id is the actual code from the SNOMED list
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// A friendly description of this ontology term used by the Tissue Directory
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// A Tissue Directory specific categorisation tag for this ontology term
        /// </summary>
        public string Tag { get; set; } = null!;
    }
}
