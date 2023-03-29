using Nest;

namespace Biobanks.Directory.Search.Dto.Documents
{
    public class OtherTermsDocument
    {
        [Keyword(Name = "name")]
        public string Name { get; set; }
    }
}
