using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Directory.Identity.Constants;
using Newtonsoft.Json;

namespace Directory.Identity
{
    public class ApplicationUserPrincipal : ClaimsPrincipal
    {
        public ApplicationUserPrincipal(IPrincipal principal) : base(principal) { }

        //Add custom claims...
        public string Name => FindFirst(CustomClaimType.FullName).Value; //ClaimTypes.Name is used for username, which is acquirable directly from the identity instead
        public string Email => FindFirst(CustomClaimType.Email).Value;
        public Dictionary<int, string> Biobanks => FindAll(CustomClaimType.Biobank).ToDictionary(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Key, x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Value);
        public Dictionary<int, string> Networks => FindAll(CustomClaimType.Network).ToDictionary(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Key, x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Value);
        public Dictionary<int, string> BiobankRequests => FindAll(CustomClaimType.BiobankRequest).ToDictionary(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Key, x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Value);
        public Dictionary<int, string> NetworkRequests => FindAll(CustomClaimType.NetworkRequest).ToDictionary(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Key, x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value).Value);
    }
}
