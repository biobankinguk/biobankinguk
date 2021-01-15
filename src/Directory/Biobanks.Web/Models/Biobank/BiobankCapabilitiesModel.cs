using System.Collections.Generic;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankCapabilitiesModel
    {
        public IEnumerable<BiobankCapabilityModel> BiobankCapabilityModels { get; set; }
    }

    public class BiobankCapabilityModel
    {
        public int Id { get; set; }
        public string SnomedTerm { get; set; }
        public string Protocol { get; set; }
    }
}