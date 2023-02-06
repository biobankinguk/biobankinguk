using Biobanks.Entities.Data.ReferenceData;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Api.ReferenceData
{
    /// <summary>
    /// Ontology Versions.
    /// </summary>
    public class OntologyVersion : BaseReferenceData
    {
        /// <summary>
        /// Ontology to which the version relates.
        /// </summary>
        [Required]
        public int OntologyId { get; set; }

        /// <summary>
        /// Ontology to which the version relates.
        /// </summary>
        [Required]
        public Ontology Ontology { get; set; }
    }
}