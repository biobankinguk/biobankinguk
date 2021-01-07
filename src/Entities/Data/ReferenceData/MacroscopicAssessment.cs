using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class MacroscopicAssessment
    {
        public int MacroscopicAssessmentId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
