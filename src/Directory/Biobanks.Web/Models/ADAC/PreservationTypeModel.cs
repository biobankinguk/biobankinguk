using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.ADAC
{
    public class PreservationTypeModel
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public int StorageTemperatureId { get; set; }

        public string StorageTemperatureName { get; set; }
    }
}