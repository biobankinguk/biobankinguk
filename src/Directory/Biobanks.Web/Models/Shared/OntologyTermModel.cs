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

    }
}