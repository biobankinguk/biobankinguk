using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;

namespace Biobanks.Submissions.Api.Auth;

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
      
      claims.AddRange(
        organisations
          .Select(x => new KeyValuePair<int, string>(x.OrganisationId, x.Name))
          .Select(x => new Claim(CustomClaimType.Biobank, JsonSerializer.Serialize(x))));
  
      claims.AddRange(
        organisationsRequests
          .Where(x => x.UserEmail == user.Email)
          .Select(x => new KeyValuePair<int, string>(x.OrganisationRegisterRequestId, x.OrganisationName))
          .Select(x => new Claim(CustomClaimType.BiobankRequest, JsonSerializer.Serialize(x))));
    }
    
    // If they're a Network Admin then populate the claim for the ID of their Network.
    // Additionally, add claims for accepted network requests
    if (principal.IsInRole(Role.NetworkAdmin))
    {
      var networks = await _networkService.ListByUserId(user.Id);

      var networkRequests = await _networkService.ListAcceptedRegistrationRequestsByUserId(user.Id);
      
      claims.AddRange(networks
        .Select(x => new KeyValuePair<int, string>(x.NetworkId, x.Name))
        .Select(x => new Claim(CustomClaimType.Network, JsonSerializer.Serialize(x))));

      claims.AddRange(networkRequests
        .Select(x => new KeyValuePair<int, string>(x.NetworkRegisterRequestId, x.NetworkName))
        .Select(x => new Claim(CustomClaimType.NetworkRequest, JsonSerializer.Serialize(x))));
    }

    identity.AddClaims(claims);
    return principal;
  }
  
}
