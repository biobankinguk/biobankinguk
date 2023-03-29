using System.Collections.Generic;
using Biobanks.Search.Dto.Documents;

namespace Biobanks.Search.Dto.PartialDocuments
{
    public class PartialNetworks
    {
        public IEnumerable<NetworkDocument> Networks { get; set; }
    }
}