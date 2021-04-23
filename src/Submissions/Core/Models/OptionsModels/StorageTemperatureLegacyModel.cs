using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Core.Models.OptionsModels
{
    
    public class StorageTemperatureLegacyModel
    {
        [JsonProperty("storageTemperatures")]
        public List<StorageTemperature> StorageTemperatures { get; set; }
    }

    public class StorageTemperature
    {
        [JsonProperty("old")]
        public Old Old { get; set; }
        [JsonProperty("new")]
        public New New { get; set; }
    }
    public class Old
    {
        [JsonProperty("storageTemperature")]
        public string StorageTemperature { get; set; }
    }

    public class New
    {
        [JsonProperty("storageTemperature")]
        public string StorageTemperature { get; set; }

        [JsonProperty("preservationType")]
        public string PreservationType { get; set; }
    }
}
