using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Biobanks.Identity.Constants;

namespace Biobanks.Identity
{
    public class ApplicationUserPrincipal : ClaimsPrincipal
    {
        public ApplicationUserPrincipal(IPrincipal principal) : base(principal) { }

        //Add custom claims...
        public string Name => FindFirst(CustomClaimType.FullName).Value; //ClaimTypes.Name is used for username, which is acquirable directly from the identity instead
        
        public IEnumerable<string> BiobankIds => FindAll(CustomClaimType.BiobankId).Select(x => x.Value);
        public IEnumerable<string> NetworkIds => FindAll(CustomClaimType.NetworkId).Select(x => x.Value);
    }
}
