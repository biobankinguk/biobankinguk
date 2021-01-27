using Nest;

namespace Biobanks.Search.Dto.Documents
{
    public class BaseDocument
    {
        [Keyword(Name = "diagnosis")]
        public string OntologyTerm { get; set; }

        [Keyword(Name = "biobankExternalId")]
        public string BiobankExternalId { get; set; }
    }
}
