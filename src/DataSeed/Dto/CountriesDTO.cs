using Newtonsoft.Json;

namespace Biobanks.DataSeed.Dto
{
    public class CountriesDTO
    {
        [JsonProperty("isCountry")]
        public bool IsCountry { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }

    }
}
