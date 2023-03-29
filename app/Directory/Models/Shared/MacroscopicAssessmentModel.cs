using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class MacroscopicAssessmentsModel
    {
        public ICollection<MacroscopicAssessmentModel> MacroscopicAssessments { get; set; }
    }

    public class MacroscopicAssessmentModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}

