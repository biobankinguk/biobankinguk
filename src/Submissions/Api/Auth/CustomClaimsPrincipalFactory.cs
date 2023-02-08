using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Biobanks.Submissions.Api.Auth;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{

  public CustomClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : base(userManager, roleManager, optionsAccessor){}
  
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
    
    identity.AddClaims(claims);
    return principal;
  }
  
}
