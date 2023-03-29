using System.Collections.Generic;
using Biobanks.Directory.Search.Dto.Documents;
using Nest;

namespace Biobanks.Directory.Search.Dto.PartialDocuments
{
    public class PartialSampleSet
    {
        [Keyword(Name = "sex")]
        public string Sex { get; set; }

        [Keyword(Name = "ageRange")]
        public string AgeRange { get; set; }

        [Keyword(Name = "ageRangeMetadata")]
        public string AgeRangeMetadata { get; set; }

        [Keyword(Name = "donorCount")]
        public string DonorCount { get; set; }

        [Keyword(Name = "donorCountMetadata")]
        public string DonorCountMetadata { get; set; }

        public IEnumerable<MaterialPreservationDetailDocument> MaterialPreservationDetails { get; set; }

        [Keyword(Name = "sampleSetSummary")]
        public string SampleSetSummary { get; set; }
    }
}
