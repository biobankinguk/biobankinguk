using Nest;

namespace Biobanks.Search.Dto.Documents
{
    public class BiobankServiceDocument
    {
        [Keyword(Name = "name")]
        public string Name { get; set; }
    }
}
