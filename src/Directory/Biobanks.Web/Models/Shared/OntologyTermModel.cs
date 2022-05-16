using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class OntologyTermModel
    {
        [Required]
        [StringLength(20)]
        public string OntologyTermId { get; set; }

        [Required]
        public string Description { get; set; }

        public string OtherTerms { get; set; }

        public bool DisplayOnDirectory { get; set; }

        public List<string> MatchingOtherTerms { get; set; }
        public List<string> NonMatchingOtherTerms { get; set; }

        public List<AssociatedDataType> AssociatedDataTypes {get; set;}
    }
}