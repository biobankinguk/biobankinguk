using System.Collections.Generic;
using Biobanks.Search.Dto.Documents;
using Nest;

namespace Biobanks.Search.Dto.PartialDocuments
{
    public class PartialCollection
    {
        [Text(Name = "ontologyTerm")]
        public string OntologyTerm { get; set; }

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

        [Keyword(Name = "collectionType")]
        public string CollectionType { get; set; }

        public IEnumerable<AssociatedDataDocument> AssociatedData { get; set; }

        [Keyword(Name = "ontologyOtherTerms")]
        public IEnumerable<OtherTermsDocument> OntologyOtherTerms { get; set; }
    }
}