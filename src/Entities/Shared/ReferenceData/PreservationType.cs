using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Shared.ReferenceData
{
    public class PreservationType
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public int? StorageTemperatureId { get; set; }

        public virtual StorageTemperature StorageTemperature { get; set; }
    }
}
