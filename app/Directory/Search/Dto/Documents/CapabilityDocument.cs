using System.Collections.Generic;
using Nest;

namespace Biobanks.Directory.Search.Dto.Documents
{
    //[ElasticsearchType(Name = "_doc")] // TODO: name is obsolete in 7. how to fix? just remove?
    public class CapabilityDocument : BaseDocument
    {
        public int Id { get; set; }

        [Keyword(Name = "biobankId")]
        public int BiobankId { get; set; }

        [Keyword(Name = "biobank")]
        public string Biobank { get; set; }

        public IEnumerable<NetworkDocument> Networks { get; set; }

        [Keyword(Name = "protocols")]
        public string Protocols { get; set; }

        [Keyword(Name = "annualDonorExpectation")]
        public string AnnualDonorExpectation { get; set; }

        [Keyword(Name = "annualDonorExpectationMetadata")]
        public string AnnualDonorExpectationMetadata { get; set; }

        public IEnumerable<AssociatedDataDocument> AssociatedData { get; set; }

        public IEnumerable<BiobankServiceDocument> BiobankServices { get; set; }

        public IEnumerable<OtherTermsDocument> OntologyOtherTerms { get; set; }
    }
}
