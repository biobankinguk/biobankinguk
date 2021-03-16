using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Biobanks.Submissions.Api.Auth
{
    public static class AuthPolicies
    {
        private static AuthorizationPolicy IsAuthenticated
            => new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

        public static AuthorizationPolicy IsTokenAuthenticated
            => new AuthorizationPolicyBuilder()
                .Combine(IsAuthenticated)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireClaim(CustomClaimTypes.BiobankId)
                .Build();

        public static AuthorizationPolicy IsBasicAuthenticated
            => new AuthorizationPolicyBuilder()
                .Combine(IsAuthenticated)
                .AddAuthenticationSchemes(BasicAuthConstants.AuthenticationScheme)
                .Build();
    }
}
