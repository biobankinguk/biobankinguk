using System;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Home
{
    public class NetworkNonMemberContactModel
    {
        public int NetworkId { get; set; }
        public List<Guid> BiobankAnonymousIdentifiers { get; set; }
    }
}
