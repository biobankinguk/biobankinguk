using Nest;

namespace Biobanks.Search.Dto.Documents
{
    public class AssociatedDataDocument
    {
        [Keyword(Name = "text")]
        public string Text { get; set; }

        [Keyword(Name = "timeframe")]
        public string Timeframe { get; set; }

        [Keyword(Name = "timeframeMetadata")]
        public string TimeframeMetadata { get; set; }
    }
}