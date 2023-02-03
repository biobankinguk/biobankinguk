using Nest;

namespace Biobanks.Search.Dto.Documents
{
    public class ConsentRestrictionDocument
    {
        [Keyword(Name = "description")]
        public string Description { get; set; }
    }
}
