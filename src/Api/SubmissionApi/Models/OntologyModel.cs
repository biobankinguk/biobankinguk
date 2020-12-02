using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.SubmissionApi.Models
{
    /// <summary>
    /// Ontology - for examples, see https://www.ebi.ac.uk/ols/ontologies.
    /// </summary>
    public class OntologyModel
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the ontology (i.e. friendly name)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Versions available for this ontology.
        /// </summary>
        public ICollection<OntologyVersionModel> OntologyVersions { get; set; }
    }
}
