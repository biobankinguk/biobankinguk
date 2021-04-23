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
        public List<StorageTemperature> ListOfSettings { get; set; }
    }

    public class StorageTemperature
    {
        public old old { get; set; }

        public @new @new { get; set; }
    }
    public class old
    {
        public string storageTemperature { get; set; }
    }

    public class @new
    {
        public string storageTemperature { get; set; }
        public string preservationType { get; set; }
    }
}
