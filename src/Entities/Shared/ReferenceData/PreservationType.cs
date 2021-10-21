using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Entities.Shared.ReferenceData
{
    public class PreservationType : BaseReferenceData
    { 
        public int? StorageTemperatureId { get; set; }

        public virtual StorageTemperature StorageTemperature { get; set; }
    }
}
