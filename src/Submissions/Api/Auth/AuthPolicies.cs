using System.Collections.Generic;
using System.Linq;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Biobanks.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Biobanks.Submissions.Api.Auth
{
    /// <summary>
    /// ASP.NET Core Authorization Policy Definitions
    /// </summary>
    public static class AuthPolicies
    {
        /// <summary>
        /// Requires that a request is authenticated
        /// </summary>
        public static AuthorizationPolicy IsAuthenticated
            => new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

        /// <summary>
        /// Requires that a request is authenticated via Bearer Token
        /// and expects all Token authenticated users to have a BiobankId claim
        /// </summary>
        public static AuthorizationPolicy IsTokenAuthenticated
            => new AuthorizationPolicyBuilder()
                .Combine(IsAuthenticated)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireClaim(CustomClaimTypes.BiobankId)
                .Build();

        /// <summary>
        /// Requires that a request is authenticated via Basic Auth
        /// </summary>
        public static AuthorizationPolicy IsBasicAuthenticated
            => new AuthorizationPolicyBuilder()
                .Combine(IsAuthenticated)
                .AddAuthenticationSchemes(BasicAuthDefaults.AuthenticationScheme)
                .Build();

        /// <summary>
        /// Requires that a request is authorised to access Hangfire functionality
        /// </summary>
        public static AuthorizationPolicy CanAccessHangfireDashboard
            => new AuthorizationPolicyBuilder()
                .Combine(IsBasicAuthenticated)
                .RequireClaim(ClaimTypes.Role, CustomRoles.SuperUser)
                .Build();

        public static AuthorizationPolicy IsSuperUser
        => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .RequireClaim(ClaimTypes.Role, CustomRoles.SuperUser)
            .Build();

        public static AuthorizationPolicy IsDirectoryAdmin
        => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .RequireClaim(ClaimTypes.Role, Role.DirectoryAdmin)
            .Build();

        public static AuthorizationPolicy IsBiobankAdmin
        => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .RequireClaim(ClaimTypes.Role, Role.BiobankAdmin)
            .Build();

        public static AuthorizationPolicy IsNetworkAdmin
        => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .RequireClaim(ClaimTypes.Role, Role.NetworkAdmin)
            .Build();        
        
        /// <summary>
        /// Requires that a request is authenticated, and is a biobankAdmin
        /// And that they have that claim to the specific biobank which is not suspended.
        /// </summary>
        public static AuthorizationPolicy HasBiobankClaim
        => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsBiobankAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              if (!int.TryParse(
                    (string?)httpContext?.Request.RouteValues.GetValueOrDefault("id") ?? string.Empty,
                    out var biobankId))
                return false;
              
              // list their biobank claims
              var biobanks = context.User.FindAll(CustomClaimType.Biobank).ToDictionary(x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Key, x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Value);

              // verify biobank claim
              if (!biobanks.ContainsKey(biobankId))
              {
                return false;
              }
              
              // get the biobank
              var organisationService = httpContext?.RequestServices.GetService<IOrganisationDirectoryService>();
              if (organisationService == null) return false;
              
              var biobank =
                Task.Run(async () =>
                    await organisationService.Get(biobankId))
                  .Result;

              //only fail if suspended
              if (biobank != null && biobank.IsSuspended)
              {
                return false;
              }
              
              return true;
            })
            .Build();
        
        /// <summary>
        /// Requires that a request is authenticated, ans is a networkAdmin
        /// And that they have that claim to the specific network
        /// </summary>
        public static AuthorizationPolicy HasNetworkClaim
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsNetworkAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("id") ?? string.Empty,
                    out var networkId))
                return false;
              
              // list their network claims
              var networks = context.User.FindAll(CustomClaimType.Network).ToDictionary(x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Key, x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Value);

              // verify biobank claim
              if (!networks.ContainsKey(networkId))
              {
                return false;
              }
              
              return true;
            })
            .Build();        
  }
}
