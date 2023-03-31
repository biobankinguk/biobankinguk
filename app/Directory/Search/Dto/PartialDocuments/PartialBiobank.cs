using System.Collections.Generic;
using Biobanks.Directory.Search.Dto.Documents;
using Nest;

namespace Biobanks.Directory.Search.Dto.PartialDocuments
{
    public class PartialBiobank
    {
        [Keyword(Name = "biobank")]
        public string Biobank { get; set; }

        public IEnumerable<NetworkDocument> Networks { get; set; }

        public IEnumerable<BiobankServiceDocument> BiobankServices { get; set; }
    }
}
