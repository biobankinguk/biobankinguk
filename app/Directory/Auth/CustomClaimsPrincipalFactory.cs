#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Auth;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
  private readonly IOrganisationDirectoryService _organisationDirectoryService;
  private readonly INetworkService _networkService;

  public CustomClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor,
    IOrganisationDirectoryService organisationDirectoryService,
    INetworkService networkService
    )
    : base(userManager, roleManager, optionsAccessor)
  {
    _organisationDirectoryService = organisationDirectoryService;
    _networkService = networkService;
  }
  
  public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
  {
    var principal = await base.CreateAsync(user);
    var identity = (ClaimsIdentity?)principal.Identity
                   ?? throw new InvalidOperationException(
                     "No ClaimsIdentity present on this user");

    List<Claim> claims = new()
    {
      new Claim(CustomClaimTypes.FullName, user.Name),
      new Claim(CustomClaimTypes.Email, user.Email),
    };
    
    // If they're a Biobank Admin then populate the claim for the ID of their Biobank.
    // Additionally, add claims for accepted biobank requests
    if (principal.IsInRole(Role.BiobankAdmin))
    {
      var organisations = await _organisationDirectoryService.ListByUserId(user.Id);
      var organisationsRequests = await _organisationDirectoryService.ListAcceptedRegistrationRequestsByUserId(user.Id);
      
      // Add claims but prevent duplicates if the user already has them. 
      var biobankClaims = organisations
        .Select(x => new KeyValuePair<int, string>(x.OrganisationId, x.Name))
        .Select(x => new Claim(CustomClaimType.Biobank, JsonSerializer.Serialize(x)))
        .Where(x => !identity.HasClaim(c => c.Type == x.Type && c.Value == x.Value));
      
      var biobankRequestClaims = organisationsRequests
        .Where(x => x.UserEmail == user.Email)
        .Select(x => new KeyValuePair<int, string>(x.OrganisationRegisterRequestId, x.OrganisationName))
        .Select(x => new Claim(CustomClaimType.BiobankRequest, JsonSerializer.Serialize(x)))
        .Where(x => !identity.HasClaim(c => c.Type == x.Type && c.Value == x.Value));
      
      claims.AddRange(biobankClaims);
      claims.AddRange(biobankRequestClaims);
    }
    
    // If they're a Network Admin then populate the claim for the ID of their Network.
    // Additionally, add claims for accepted network requests
    if (principal.IsInRole(Role.NetworkAdmin))
    {
      var networks = await _networkService.ListByUserId(user.Id);
      var networkRequests = await _networkService.ListAcceptedRegistrationRequestsByUserId(user.Id);
      
      // Add claims but prevent duplicates if the user already has them.
      var networkClaims = networks
        .Select(x => new KeyValuePair<int, string>(x.NetworkId, x.Name))
        .Select(x => new Claim(CustomClaimType.Network, JsonSerializer.Serialize(x)))
        .Where(x => !identity.HasClaim(c => c.Type == x.Type && c.Value == x.Value));      
      
      var networkRequestClaims = networkRequests
        .Select(x => new KeyValuePair<int, string>(x.NetworkRegisterRequestId, x.NetworkName))
        .Select(x => new Claim(CustomClaimType.NetworkRequest, JsonSerializer.Serialize(x)))
        .Where(x => !identity.HasClaim(c => c.Type == x.Type && c.Value == x.Value));
      
      claims.AddRange(networkClaims);
      claims.AddRange(networkRequestClaims);
    }

    identity.AddClaims(claims);
    return principal;
  }
  
}
