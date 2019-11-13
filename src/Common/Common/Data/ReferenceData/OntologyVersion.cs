using System.ComponentModel.DataAnnotations;

namespace Common.Data.ReferenceData
{
    //TODO this might be removed when we transition fully over to OMOP. Currently being kept to preserve structure of migrated entities.
    public class OntologyVersion : BaseReferenceDatum
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
        public Ontology Ontology { get; set; } = null!;
    }
}
