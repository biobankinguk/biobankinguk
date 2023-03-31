using System.Collections.Generic;
using Nest;

namespace Biobanks.Directory.Search.Dto.Documents
{
    //[ElasticsearchType(Name = "_doc")] // TODO: name is obsolete in 7. how to fix? just remove?
    public class CollectionDocument : BaseDocument
    {
        public int Id { get; set; }

        [Keyword(Name = "biobankId")]
        public int BiobankId { get; set; }

        [Keyword(Name = "biobank")]
        public string Biobank { get; set; }

        public IEnumerable<NetworkDocument> Networks { get; set; }

        [Keyword(Name = "collectionId")]
        public int CollectionId { get; set; }

        [Keyword(Name = "collectionTitle")]
        public string CollectionTitle { get; set; }

        [Keyword(Name = "startYear")]
        public string StartYear { get; set; }

        [Keyword(Name = "endYear")]
        public string EndYear { get; set; }

        [Keyword(Name = "collectionStatus")]
        public string CollectionStatus { get; set; }

        public IEnumerable<ConsentRestrictionDocument> ConsentRestrictions { get; set; }

        [Keyword(Name = "accessCondition")]
        public string AccessCondition { get; set; }

        [Keyword(Name = "accessConditionMetadata")]
        public string AccessConditionMetadata { get; set; }

        [Keyword(Name = "collectionType")]
        public string CollectionType { get; set; }

        public IEnumerable<AssociatedDataDocument> AssociatedData { get; set; }

        [Keyword(Name = "ageRange")]
        public string AgeRange { get; set; }

        [Keyword(Name = "ageRangeMetadata")]
        public string AgeRangeMetadata { get; set; }

        [Keyword(Name = "donorCount")]
        public string DonorCount { get; set; }

        [Keyword(Name = "donorCountMetadata")]
        public string DonorCountMetadata { get; set; }

        [Keyword(Name = "sex")]
        public string Sex { get; set; }

        [Keyword(Name = "sexMetadata")]
        public string SexMetadata { get; set; }

        public IEnumerable<MaterialPreservationDetailDocument> MaterialPreservationDetails { get; set; }

        public IEnumerable<BiobankServiceDocument> BiobankServices { get; set; }

        [Keyword(Name = "sampleSetSummary")]
        public string SampleSetSummary { get; set; }

        [Keyword(Name = "country")]
        public string Country { get; set; }

        [Keyword(Name = "county")]
        public string County { get; set; }

        [Keyword(Name = "ontologyOtherTerms")]
        public IEnumerable<OtherTermsDocument> OntologyOtherTerms { get; set; }
    }
}
