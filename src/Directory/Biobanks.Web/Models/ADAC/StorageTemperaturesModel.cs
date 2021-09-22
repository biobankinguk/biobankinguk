using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.ADAC
{
    public class StorageTemperaturesModel
    {
        public ICollection<StorageTemperatureModel> StorageTemperatures { get; set; }
    }

    public class StorageTemperatureModel
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public bool InUse { get; set; }
    }
}