using Nest;

namespace Directory.Search.Dto.Documents
{
    public class BaseDocument
    {
        [Keyword(Name = "diagnosis")]
        public string Diagnosis { get; set; }

        [Keyword(Name = "biobankExternalId")]
        public string BiobankExternalId { get; set; }
    }
}
