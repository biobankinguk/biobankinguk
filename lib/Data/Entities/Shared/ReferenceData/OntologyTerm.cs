using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Shared.ReferenceData
{
    /// <summary>
    /// Represents a term from the SNOMED list.
    /// </summary>
    public class OntologyTerm
    {
        /// <summary>
        /// This id is the actual code from the SNOMED list
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(20)]
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// A friendly description of this SNOMED term used by Biobanks Phase 2.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Directory Alias For Search
        /// </summary>
        public string OtherTerms { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? SnomedTagId { get; set; }
        
        /// <summary>
        /// A Biobanks Phase 2 specific "category" tag.
        /// </summary>
        public SnomedTag SnomedTag { get; set; }

        /// <summary>
        /// Whether this term should be visible on the Directory
        /// </summary>
        public bool DisplayOnDirectory { get; set; }

        /// <summary>
        /// Join entities for MaterialTypes in the group
        /// </summary>
        public ICollection<MaterialType> MaterialTypes { get; set; }

        public List<AssociatedDataType> AssociatedDataTypes { get; set; }
    }
}
