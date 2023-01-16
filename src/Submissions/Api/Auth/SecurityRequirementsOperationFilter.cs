using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Auth
{
    /// <summary>
    /// Swashbuckle Filter to apply OpenAPI Security Definitions to particular matching Controller Actions
    /// </summary>
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        const string _defaultScheme = "jwtbearer";

        private static Dictionary<string, string> _policySchemes = new()
        {
            [nameof(AuthPolicies.IsTokenAuthenticated)] = "jwtbearer",
            [nameof(AuthPolicies.IsBasicAuthenticated)] = "basic",
            [nameof(AuthPolicies.CanAccessHangfireDashboard)] = "basic",
        };

        private IEnumerable<string> GetDistinctAuthorizationPolicyNames(object[] attributes)
            => attributes.OfType<AuthorizeAttribute>().Select(a => a.Policy).Distinct();

        /// <summary>
        /// Apply OpenAPI Security Requirements based on some filtering logic
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // TODO: if we ever add any public Controller actions we'll need to account for [AllowAnonymous]

            // Policy names map to Authentication Schemes
            var authPolicies =
                GetDistinctAuthorizationPolicyNames(
                    // get attributes on the method
                    context.MethodInfo.GetCustomAttributes(true))
                .Union(
                    // get attributes on the parent class
                    GetDistinctAuthorizationPolicyNames(
                        context.MethodInfo.ReflectedType.GetCustomAttributes(true)))
                .ToList();

            // always use the default
            if (authPolicies.Count == 0) authPolicies = new() { nameof(AuthPolicies.IsTokenAuthenticated) };

            // saves adding these per Endpoint!
            // TODO: allow overriding on the Controller Action?
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            // Generate a Security Requirement for each policy we care about
            operation.Security = authPolicies
                // map policies to auth schemes
                .Select(policyName =>
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = _policySchemes.GetValueOrDefault(policyName) ?? _defaultScheme // unknown policies use the default scheme
                        }
                    })
                // filter out policies we don't care about
                .Where(x => x is not null)
                // filter out duplicates
                .ToHashSet()
                // turn each unique scheme into a security requirement
                .Select(scheme =>
                    new OpenApiSecurityRequirement
                    {
                        [scheme] = new List<string>()
                    })
                .ToList();

        }
    }
}
