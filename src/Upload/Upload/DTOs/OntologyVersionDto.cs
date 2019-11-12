namespace Upload.DTOs
{
    /// <summary>
    /// Ontology version.
    /// </summary>
    public class OntologyVersionDto
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the ontology version (i.e. friendly name)
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Ontology to which the version relates.
        /// </summary>
        public string OntologyValue { get; set; } = string.Empty;
    }
}
