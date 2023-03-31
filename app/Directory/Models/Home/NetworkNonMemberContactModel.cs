using System;
using System.Collections.Generic;

namespace Biobanks.Directory.Models.Home
{
    public class NetworkNonMemberContactModel
    {
        public int NetworkId { get; set; }
        public List<Guid> BiobankAnonymousIdentifiers { get; set; }
    }
}
