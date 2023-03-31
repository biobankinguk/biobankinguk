using System.Collections.Generic;
using Biobanks.Directory.Search.Dto.Documents;

namespace Biobanks.Directory.Search.Dto.PartialDocuments
{
    public class PartialNetworks
    {
        public IEnumerable<NetworkDocument> Networks { get; set; }
    }
}
