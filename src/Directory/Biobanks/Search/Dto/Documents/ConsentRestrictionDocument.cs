using Nest;

namespace Directory.Search.Dto.Documents
{
    public class ConsentRestrictionDocument
    {
        [Keyword(Name = "description")]
        public string Description { get; set; }
    }
}
