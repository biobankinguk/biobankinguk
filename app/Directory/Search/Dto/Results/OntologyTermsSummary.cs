using System.Collections.Generic;

namespace Biobanks.Directory.Search.Dto.Results
{
    public class OntologyTermsSummary
    {
        public string OntologyTerm { get; set; }
        public List<string> MatchingOtherTerms { get; set; }
    }
}
