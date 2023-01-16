﻿using Biobanks.Submissions.Api.Auth.Basic;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

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
    }
}
