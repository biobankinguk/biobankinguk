using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.ADAC
{
    public class PreservationTypesModel
    {
        public ICollection<PreservationTypeModel> PreservationTypes { get; set; }

        public IEnumerable<StorageTemperature> StorageTemperatures { get; set; }
    }

    public class PreservationTypeModel
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public int? StorageTemperatureId { get; set; }

        public string StorageTemperatureName { get; set; }

        public int PreservationTypeCount { get; set; }

        public bool DefaultValue { get; set; }
    }
}