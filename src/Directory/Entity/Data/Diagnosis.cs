using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Directory.Entity.Data
{
    public class Diagnosis
    {
        public int DiagnosisId { get; set; }

        public string OtherTerms { get; set; }

        [MaxLength(20)]
        public string SnomedIdentifier { get; set; }

        [Required]
        [Index("IX_UniqueDiagnosisDescription", IsUnique = true)]
        [MaxLength(200)]
        public string Description { get; set; }

        //1 -> M Navigation properties?
        //Capabilities
        //Collections
    }
}
