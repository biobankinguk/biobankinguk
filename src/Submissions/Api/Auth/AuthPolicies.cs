using System.Collections.Generic;
using System.Linq;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
                    (string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
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
              if (organisationService is null) return false;

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
        /// Requires that a request is authenticated, and is a networkAdmin
        /// And that they have that claim to the specific network
        /// </summary>
        public static AuthorizationPolicy HasNetworkClaim
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsNetworkAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;

              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
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
        
        /// <summary>
        /// Requires that a request is authenticated, and HasBiobankClaim
        /// And that the biobank can administer the sample set
        /// </summary>
        public static AuthorizationPolicy CanAdministerSampleSet
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(HasBiobankClaim)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              // get the biobank
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
                    out var biobankId))
                return false;              
              
              // verify sample set
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("id") ?? string.Empty,
                    out var sampleSetId))
                return false;
              
              var biobankReadService = httpContext?.RequestServices.GetService<IBiobankReadService>();
              if (biobankReadService is null) 
                return false;
              
              if (!biobankReadService.CanThisBiobankAdministerThisSampleSet(biobankId, sampleSetId))
                return false;
              
              return true;
            })
            .Build();
        
        /// <summary>
        /// Requires that a request is authenticated, and HasBiobankClaim
        /// And that the biobank can administer the capability
        /// </summary>
        public static AuthorizationPolicy CanAdministerCapability
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(HasBiobankClaim)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              // get the biobank
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
                    out var biobankId))
                return false;              
              
              // verify capability
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("id") ?? string.Empty,
                    out var capabilityId))
                return false;
              
              var biobankReadService = httpContext?.RequestServices.GetService<IBiobankReadService>();
              if (biobankReadService is null)
                return false;
              
              if (!biobankReadService.CanThisBiobankAdministerThisCapability(biobankId, capabilityId))
                return false;
              
              return true;
            })
            .Build();
        
        /// <summary>
        /// Requires that a request is authenticated, and HasBiobankClaim
        /// And that the biobank can administer the collection
        /// </summary>
        public static AuthorizationPolicy CanAdministerCollection
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(HasBiobankClaim)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              // get the biobank
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty, 
                    out var biobankId))
                return false;              
              
              // verify collection
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("id") ?? string.Empty, 
                    out var collectionId))
                return false;
              
              var biobankReadService = httpContext?.RequestServices.GetService<IBiobankReadService>();
              if (biobankReadService is null)
                return false;
              
              if (!biobankReadService.CanThisBiobankAdministerThisCollection(biobankId, collectionId))
                return false;
              
              return true;
            })
            .Build();

  }
}
