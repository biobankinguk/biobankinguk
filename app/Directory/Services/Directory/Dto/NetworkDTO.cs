using System;

namespace Biobanks.Directory.Services.Directory.Dto
{
    public class NetworkDTO
    {
        public int NetworkId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Url { get; set; }

        public string Logo { get; set; } //Filepath as per Organisation

        public string Description { get; set; }

        public int SopStatusId { get; set; }

        public DateTime? LastUpdated { get; set; }

        // Handing over requests to third parties
        public bool ContactHandoverEnabled { get; set; }

        public string HandoverBaseUrl { get; set; }

        public string HandoverOrgIdsUrlParamName { get; set; }

        public bool MultipleHandoverOrdIdsParams { get; set; }

        public bool HandoverNonMembers { get; set; }

        public string HandoverNonMembersUrlParamName { get; set; }
    }
}
