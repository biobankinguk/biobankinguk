using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Directory.Identity.Data.Entities;
using Directory.Identity.Services;
using Directory.Identity.Contracts;
using Directory.Identity.Constants;
using Directory.Services.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Directory.Services
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
                new Claim(CustomClaimType.FullName, user.Name)
            };

            // If they're a Biobank Admin then populate the claim for the ID of their Biobank.
            if ((await _userManager.GetRolesAsync(user.Id)).Any(x => x == Role.BiobankAdmin.ToString()))
            {
                var biobanks = _biobankReadService.GetBiobankIdsAndNamesByUserId(user.Id);

                claims.AddRange(biobanks.Select(biobank => new Claim(CustomClaimType.BiobankId, biobank.Key.ToString())));
            }

            // If they're a Network Admin then populate the claim for the ID of their Network.
            if ((await _userManager.GetRolesAsync(user.Id)).Any(x => x == Role.NetworkAdmin.ToString()))
            {
                var networks = _biobankReadService.GetNetworkIdsAndNamesByUserId(user.Id);

                claims.AddRange(networks.Select(network => new Claim(CustomClaimType.NetworkId, network.Key.ToString())));
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
