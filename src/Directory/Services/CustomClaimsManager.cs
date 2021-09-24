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
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Services
{
    public class CustomClaimsManager
    {
        private INetworkService _networkService;
        private IOrganisationService _organisationService;
        
        private readonly ApplicationSignInManager _signinManager;
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IBiobankReadService _biobankReadService;

        public CustomClaimsManager(
            INetworkService networkService,
            IOrganisationService organisationService,
            ApplicationSignInManager signinManager,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IBiobankReadService biobankReadService)
        {
            _networkService = networkService;
            _organisationService = organisationService;
            _signinManager = signinManager;
            _userManager = userManager;
            _biobankReadService = biobankReadService;
        }


        public async Task SetUserClaimsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var userRoles = await _userManager.GetRolesAsync(user.Id);

            var claims = new List<Claim>
            {
                new Claim(CustomClaimType.FullName, user.Name),
                new Claim(CustomClaimType.Email, user.Email)
            };

            // If they're a Biobank Admin then populate the claim for the ID of their Biobank.
            // Additionally, add claims for accepted biobank requests
            if (userRoles.Any(x => x == Role.BiobankAdmin.ToString()))
            {
                var organisations = await _organisationService.ListByUserId(user.Id);
                var organisationsRequests = await _organisationService.ListAcceptedRegistrationRequests();
                
                claims.AddRange(
                    organisations
                        .Select(x => new KeyValuePair<int, string>(x.OrganisationId, x.Name))
                        .Select(x => new Claim(CustomClaimType.Biobank, JsonConvert.SerializeObject(x))));
                
                claims.AddRange(
                    organisationsRequests
                        .Where(x => x.UserEmail == user.Email)
                        .Select(x => new KeyValuePair<int, string>(x.OrganisationRegisterRequestId, x.OrganisationName))
                        .Select(x => new Claim(CustomClaimType.BiobankRequest, JsonConvert.SerializeObject(x))));
            }

            // If they're a Network Admin then populate the claim for the ID of their Network.
            // Additionally, add claims for accepted network requests
            if (userRoles.Any(x => x == Role.NetworkAdmin.ToString()))
            {
                var networks = await _networkService.ListByUserId(user.Id);
                var networkRequests = await _networkService.ListAcceptedRegistrationRequestsByUserId(user.Id);
                
                claims.AddRange(networks
                    .Select(x => new KeyValuePair<int, string>(x.NetworkId, x.Name))
                    .Select(x => new Claim(CustomClaimType.Network, JsonConvert.SerializeObject(x))));

                claims.AddRange(networkRequests
                    .Select(x => new KeyValuePair<int, string>(x.NetworkRegisterRequestId, x.NetworkName))
                    .Select(x => new Claim(CustomClaimType.NetworkRequest, JsonConvert.SerializeObject(x))));
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
