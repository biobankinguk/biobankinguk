using System.ComponentModel.DataAnnotations;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Api.ReferenceData
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
