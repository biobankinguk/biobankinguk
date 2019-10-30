using Common.Data.ReferenceData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Upload.Common.Data.Entities
{
    //TODO this might be removed when we transition fully over to OMOP. Currently being kept to preserve structure of migrated entities.
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
        [Required]
        public Ontology Ontology { get; set; }
    }
}
