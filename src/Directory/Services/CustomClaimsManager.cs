using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Identity.Data.Entities;
using Biobanks.Identity.Services;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Constants;
using Biobanks.Services.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

namespace Biobanks.Services
{
    public class CustomClaimsManager
    {
        private readonly ApplicationSignInManager _signinManager;
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IBiobankReadService _biobankReadService;

        public CustomClaimsManager(
            ApplicationSignInManager signinManager,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IBiobankReadService biobankReadService)
        {
            _signinManager = signinManager;
            _userManager = userManager;
            _biobankReadService = biobankReadService;
        }


        public async Task SetUserClaimsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var claims = new List<Claim>
            {
                new Claim(CustomClaimType.FullName, user.Name),
                new Claim(CustomClaimType.Email, user.Email)
            };

            // If they're a Biobank Admin then populate the claim for the ID of their Biobank.
            // Additionally, add claims for accepted biobank requests
            if ((await _userManager.GetRolesAsync(user.Id)).Any(x => x == Role.BiobankAdmin.ToString()))
            {
                var biobanks = _biobankReadService.GetBiobankIdsAndNamesByUserId(user.Id);
                claims.AddRange(biobanks.Select(biobank => new Claim(CustomClaimType.Biobank, JsonConvert.SerializeObject(biobank))));
                
                var biobankRequests = _biobankReadService.GetAcceptedBiobankRequestIdsAndNamesByUserId(user.Id);
                claims.AddRange(biobankRequests.Select(biobankRequest => new Claim(CustomClaimType.BiobankRequest, JsonConvert.SerializeObject(biobankRequest))));
            }

            // If they're a Network Admin then populate the claim for the ID of their Network.
            // Additionally, add claims for accepted network requests
            if ((await _userManager.GetRolesAsync(user.Id)).Any(x => x == Role.NetworkAdmin.ToString()))
            {
                var networks = _biobankReadService.GetNetworkIdsAndNamesByUserId(user.Id);
                claims.AddRange(networks.Select(network => new Claim(CustomClaimType.Network, JsonConvert.SerializeObject(network))));

                var networkRequests = _biobankReadService.GetAcceptedNetworkRequestIdsAndNamesByUserId(user.Id);
                claims.AddRange(networkRequests.Select(networkRequest => new Claim(CustomClaimType.NetworkRequest, JsonConvert.SerializeObject(networkRequest))));
            }

            AddClaims(claims);
        }

        public void AddClaims(IEnumerable<Claim> claims)
        {
            //If we didn't perform sign in this request, we won't have an AuthenticationResponseGrant available
            //So we'll need to set a new one based on the current signed-in ClaimsPrincipal
            if (_signinManager.AuthenticationManager.AuthenticationResponseGrant == null)
                _signinManager.AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(
                    _signinManager.AuthenticationManager.User, new AuthenticationProperties {IsPersistent = true});

            //now force refresh the auth cookie (and therefore claims and role membership) BEFORE the next request
            _signinManager.AuthenticationManager.AuthenticationResponseGrant.Identity.AddClaims(claims);
        }
    }
}
