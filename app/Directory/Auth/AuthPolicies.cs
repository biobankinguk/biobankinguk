#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Biobanks.Directory.Auth.Basic;
using Biobanks.Directory.Config;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Auth
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
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="IsBiobankAdmin"/>,
        /// and has a claim to the specific biobank request.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
        public static AuthorizationPolicy HasBiobankRequestClaim
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsBiobankAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
                    out var requestId))
                return false;
              
              // list their biobank request claims
              var biobanks = context.User.FindAll(CustomClaimType.BiobankRequest).ToDictionary(x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Key, x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Value);

              // verify biobank request claim
              if (!biobanks.ContainsKey(requestId))
                return false;

              return true;
            })
            .Build();

        /// <summary>
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="IsBiobankAdmin"/>,
        /// and has a claim to the specific biobank.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
        public static AuthorizationPolicy HasBiobankClaim
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsBiobankAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
                    out var biobankId))
                return false;
              
              // list their biobank claims
              var biobanks = context.User.FindAll(CustomClaimType.Biobank).ToDictionary(x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Key, x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Value);
              
              // verify biobank claim
              if (!biobanks.ContainsKey(biobankId))
                return false;

              // check if we allow suspended biobanks
              var siteOptions = httpContext?.RequestServices.GetService<IOptions<SitePropertiesOptions>>();
              if (siteOptions != null && !siteOptions.Value.AllowSuspendedBiobanks)
              {
                // get the biobank
                var organisationService = httpContext?.RequestServices.GetService<IOrganisationDirectoryService>();
                if (organisationService is null)
                  return false;

                var biobank = Task.Run(async () => await organisationService.Get(biobankId)).Result;

                //only fail if suspended
                if (biobank is { IsSuspended: true })
                  return false;
              }

              return true;
            })
            .Build();
        
        /// <summary>
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="IsNetworkAdmin"/>,
        /// and has a claim to the specific network request.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
        public static AuthorizationPolicy HasNetworkRequestClaim
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsNetworkAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;
              
              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("networkId") ?? string.Empty,
                    out var requestId))
                return false;
              
              // list their network request claims
              var networks = context.User.FindAll(CustomClaimType.NetworkRequest).ToDictionary(x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Key, x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Value);

              // verify network request claim
              if (!networks.ContainsKey(requestId))
                return false;

              return true;
            })
            .Build();

        /// <summary>
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="IsNetworkAdmin"/>,
        /// and has a claim to the specific network.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
        public static AuthorizationPolicy HasNetworkClaim
          => new AuthorizationPolicyBuilder()
            .Combine(IsAuthenticated)
            .Combine(IsNetworkAdmin)
            .RequireAssertion(context =>
            {
              var httpContext = (DefaultHttpContext?)context.Resource;

              if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("networkId") ?? string.Empty,
                    out var networkId))
                return false;

              // list their network claims
              var networks = context.User.FindAll(CustomClaimType.Network).ToDictionary(x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Key, x => JsonSerializer
                .Deserialize<KeyValuePair<int, string>>(x.Value).Value);

              // verify biobank claim
              if (!networks.ContainsKey(networkId))
                return false;

              return true;
            })
            .Build();
        
        /// <summary>
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="HasBiobankClaim"/>,
        /// and the biobank can administer the sample set.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
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
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="HasBiobankClaim"/>,
        /// and the biobank can administer the capability.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
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
        /// Requires a request <see cref="IsAuthenticated"/>, <see cref="HasBiobankClaim"/>,
        /// and the biobank can administer the collection.
        /// </summary>
        /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
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
