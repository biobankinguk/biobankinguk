using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Core.Config.LegacySupport
{
    public class StorageTemperatureLegacyModel
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
