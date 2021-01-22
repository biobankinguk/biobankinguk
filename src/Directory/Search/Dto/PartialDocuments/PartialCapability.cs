using System.Collections.Generic;
using Biobanks.Search.Dto.Documents;
using Nest;

namespace Biobanks.Search.Dto.PartialDocuments
{
    public class PartialCapability
    {
        [Text(Name = "diagnosis")]
        public string SnomedTerm { get; set; }

        [Keyword(Name = "protocols")]
        public string Protocols { get; set; }

        [Keyword(Name = "annualDonorExpectation")]
        public string AnnualDonorExpectation { get; set; }

        [Keyword(Name = "annualDonorExpectationMetadata")]
        public string AnnualDonorExpectationMetadata { get; set; }

        public IEnumerable<AssociatedDataDocument> AssociatedData { get; set; }
    }
}
