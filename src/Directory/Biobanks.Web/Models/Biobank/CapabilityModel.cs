using System.Collections.Generic;

namespace Biobanks.Web.Models.Biobank
{
    public class CapabilityModel
    {
        public int Id { get; set; }
        public string SnomedTerm { get; set; }
        public string Protocols { get; set; }
        public int AnnualDonorExpectation { get; set; }

        public IEnumerable<AssociatedDataSummaryModel> AssociatedData { get; set; }
    }
}