using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
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

        public List<string> MatchingOtherTerms { get; set; } = new List<string>();
        public List<string> NonMatchingOtherTerms { get; set; } = new List<string>();

        public List<AssociatedDataTypeModel> AssociatedDataTypes { get; set; }
        public string AssociatedDataTypesJson { get; set; }
    }
}

