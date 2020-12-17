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

        public IEnumerable<string> BiobankIds => FindAll(CustomClaimType.Biobank).Select(x => JsonConvert.DeserializeObject<KeyValuePair<string, string>>(x.Value).Key);
 
        public IEnumerable<string> NetworkIds => FindAll(CustomClaimType.Network).Select(x => JsonConvert.DeserializeObject<KeyValuePair<string, string>>(x.Value).Key);
        public IEnumerable<KeyValuePair<int, string>> Biobanks => FindAll(CustomClaimType.Biobank).Select(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value));
        public IEnumerable<KeyValuePair<int, string>> Networks => FindAll(CustomClaimType.Network).Select(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value));
        public IEnumerable<KeyValuePair<int, string>> BiobankRequests => FindAll(CustomClaimType.BiobankRequest).Select(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value));
        public IEnumerable<KeyValuePair<int, string>> NetworkRequests => FindAll(CustomClaimType.NetworkRequest).Select(x => JsonConvert.DeserializeObject<KeyValuePair<int, string>>(x.Value));
    }
}
