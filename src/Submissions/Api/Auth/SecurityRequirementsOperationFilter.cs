using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Auth
{
    // These filters tell Swagger which security definitions (e.g. Basic Auth...) apply to which Authorization Policies

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        private static Dictionary<string, string> _policySchemes = new()
        {
            [nameof(AuthPolicies.IsTokenAuthenticated)] = "jwtbearer",
            [nameof(AuthPolicies.IsBasicAuthenticated)] = "basic"
        };

        private IEnumerable<string> GetDistinctAuthorizationPolicyNames(object[] attributes)
            => attributes.OfType<AuthorizeAttribute>().Select(a => a.Policy).Distinct();

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
            if (!authPolicies.Any()) authPolicies = new() { nameof(AuthPolicies.IsTokenAuthenticated) };

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
                            Id = _policySchemes.GetValueOrDefault(policyName)
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
