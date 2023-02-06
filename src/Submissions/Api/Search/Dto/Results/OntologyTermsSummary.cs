using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Search.Dto.Results
{
    public class OntologyTermsSummary
    {
        public string OntologyTerm { get; set; }
        public List<string> MatchingOtherTerms { get; set; }
    }
}
