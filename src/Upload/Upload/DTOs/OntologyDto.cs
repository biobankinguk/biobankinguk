using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upload.DTOs
{
    /// <summary>
    /// Ontology - for examples, see https://www.ebi.ac.uk/ols/ontologies.
    /// </summary>
    public class OntologyDto
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the ontology (i.e. friendly name)
        /// </summary>
        [Required]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Versions available for this ontology.
        /// </summary>
        public ICollection<OntologyVersionDto> OntologyVersions { get; set; } = new List<OntologyVersionDto>();
    }
}
