using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class StorageTemperatureModel
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        //public int SampleSetsCount { get; set; }

        //public bool IsInUse { get; set; }
    }
}
