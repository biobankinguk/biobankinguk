using System;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Home
{
    public class ContactModel
    {
        public IEnumerable<ContactBiobankModel> Contacts { get; set; }
        public IEnumerable<NetworkHandoverModel> Networks { get; set; }
    }
    public class ContactBiobankModel
    {
        public string BiobankExternalId { get; set; }
        public string BiobankName { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
    }

    public class NetworkHandoverModel
    {
        public int NetworkId { get; set; }
        public string NetworkName { get; set; }
        public string LogoName { get; set; }
        public string HandoverBaseUrl { get; set; }
        public string HandoverOrgIdsUrlParamName { get; set; }
        public bool MultipleHandoverOrdIdsParams { get; set; }
        public List<string> BiobankExternalIdsInNetwork { get; set; } = new List<string>();

        public bool HandoverNonMembers { get; set; }
        public string HandoverNonMembersUrlParamName { get; set; }
        public List<Guid> NonMemberBiobankAnonymousIds { get; set; } = new List<Guid>();
    }
}
