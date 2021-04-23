using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Core.Models.OptionsModels
{

    public class StorageTemperatureLegacyModel
    {
        [JsonProperty("listOfMappings")]
        public List<StorageTemperature> ListOfMappings { get; set; }
    }

    public class StorageTemperature
    {
        [JsonProperty("listOfMappings")]
        public Old Old { get; set; }
        [JsonProperty("listOfMappings")]
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
