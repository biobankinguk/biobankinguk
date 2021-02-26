using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Dto
{
    public class OntologyTermResultDTO
    {
        [Required]
        [StringLength(20)]
        public string Id { get; set; }

        [Required]
        public string Value { get; set; }

        public string OtherTerms { get; set; }

        public List<string> MatchingOtherTerms { get; set; }
        public List<string> NonMatchingOtherTerms { get; set; }
    }
}
