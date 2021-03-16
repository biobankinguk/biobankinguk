using Microsoft.AspNetCore.Authorization;

namespace Biobanks.Common.Auth
{
    public static class AuthPolicies
    {
        public static AuthorizationPolicy BuildDefaultJwtPolicy()
            => new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(
                    ctx => ctx.User.HasClaim(claim => claim.Type == CustomClaimTypes.BiobankId)
                           || ctx.User.IsInRole(CustomRoles.SuperAdmin))
                .Build();

        public static AuthorizationPolicy RequireSuperAdminRole()
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole(CustomRoles.SuperAdmin)
            .Build();
    }
}
