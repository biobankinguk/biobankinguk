using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory.DataSeed.Dto
{
    public class CountriesDTO
    {
        [JsonProperty("isCountry")]
        public bool IsCountry { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }

    }
}
