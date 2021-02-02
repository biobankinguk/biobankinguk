using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Api.ReferenceData
{
    /// <summary>
    /// Ontology Versions.
    /// </summary>
    public class OntologyVersion
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the ontology version (i.e. friendly name)
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Ontology to which the version relates.
        /// </summary>
        [Required]
        public int OntologyId { get; set; }

        /// <summary>
        /// Ontology to which the version relates.
        /// </summary>
        public virtual Ontology Ontology { get; set; }
    }
}