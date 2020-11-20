using Nest;

namespace Directory.Search.Dto.Documents
{
    public class NetworkDocument
    {
        [Keyword(Name = "name")]
        public string Name { get; set; }
    }
}
