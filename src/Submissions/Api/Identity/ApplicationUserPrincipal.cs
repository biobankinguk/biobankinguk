using Biobanks.Submissions.Api.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace Biobanks.Submissions.Api.Identity
{
    public class ApplicationUserPrincipal : ClaimsPrincipal
    {
        public ApplicationUserPrincipal(IPrincipal principal) : base(principal) { }

        //Add custom claims...
        public string Name => FindFirst(CustomClaimType.FullName).Value; //ClaimTypes.Name is used for username, which is acquirable directly from the identity instead
        public string Email => FindFirst(CustomClaimType.Email).Value;
        public Dictionary<int, string> Biobanks => CreateClaimsDictionary(CustomClaimType.Biobank);
        public Dictionary<int, string> Networks => CreateClaimsDictionary(CustomClaimType.Network);
        public Dictionary<int, string> BiobankRequests => CreateClaimsDictionary(CustomClaimType.BiobankRequest);
        public Dictionary<int, string> NetworkRequests => CreateClaimsDictionary(CustomClaimType.NetworkRequest);

        /// <summary>
        /// Return all claims of a given type as a dictionary containing Id and Name
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        private Dictionary<int, string> CreateClaimsDictionary(string claimType)
            => FindAll(claimType).ToDictionary(x => 
            JsonSerializer.Deserialize<KeyValuePair<int, string>>(x.Value).Key,
                x => JsonSerializer.Deserialize<KeyValuePair<int, string>>(x.Value).Value);
    }
}
