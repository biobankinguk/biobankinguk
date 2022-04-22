using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class PreservationTypeModel
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public int? StorageTemperatureId { get; set; }

        public string StorageTemperatureName { get; set; }

        //public int PreservationTypeCount { get; set; }
    }
}
