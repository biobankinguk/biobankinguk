using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Shared.ReferenceData
{
    public class PreservationType : BaseReferenceData
    { 
        public int? StorageTemperatureId { get; set; }

        public virtual StorageTemperature StorageTemperature { get; set; }
    }
}
