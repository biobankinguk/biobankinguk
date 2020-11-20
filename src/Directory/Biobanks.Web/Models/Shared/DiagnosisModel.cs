using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class DiagnosisModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SnomedIdentifier { get; set; }

        [Required]
        public string Description { get; set; }

        public string OtherTerms { get; set; }

    }
}