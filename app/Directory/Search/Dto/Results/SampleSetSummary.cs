using System.Collections.Generic;

namespace Biobanks.Directory.Search.Dto.Results
{
    public class SampleSetSummary
    {
        public SampleSetSummary()
        {
            MaterialPreservationDetails = new List<MaterialPreservationDetailSummary>();
        }

        public string Sex { get; set; }
        public string AgeRange { get; set; }
        public string DonorCount { get; set; }

        public IEnumerable<MaterialPreservationDetailSummary> MaterialPreservationDetails { get; set; }
    }
}
