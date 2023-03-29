using System.Collections.Generic;

namespace Biobanks.Directory.Search.Dto.Results
{
    public class BiobankCapabilityResult
    {
        public BiobankCapabilityResult()
        {
            Capabilities = new List<CapabilitySummary>();
        }

        public int BiobankId { get; set; }
        public string BiobankExternalId { get; set; }
        public string BiobankName { get; set; }

        public IList<CapabilitySummary> Capabilities { get; set; }
    }
}
