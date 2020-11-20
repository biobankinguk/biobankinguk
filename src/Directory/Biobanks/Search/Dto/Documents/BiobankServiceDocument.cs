using Nest;

namespace Directory.Search.Dto.Documents
{
    public class BiobankServiceDocument
    {
        [Keyword(Name = "name")]
        public string Name { get; set; }
    }
}
