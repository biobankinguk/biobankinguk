using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class DiagnosisModel
    {
        [Required]
        [StringLength(20)]
        public string SnomedTermId { get; set; }

        [Required]
        public string Description { get; set; }

        public string OtherTerms { get; set; }

    }
}