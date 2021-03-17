using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Config;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Swashbuckle.AspNetCore.Annotations;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = nameof(AuthPolicies.IsBasicAuthenticated))]
    public class TokenController : ControllerBase
    {
        private readonly JwtBearerConfig _config;

        public TokenController(IOptions<JwtBearerConfig> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Request an Submissions API Access JWT for a set of client credentials.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.TJVA95OrM7E2cBab30RMHrHDcEfxjoYZgeFONFh7HgQ</example>
        /// <returns>A paginated list of submission summaries.</returns>
        [HttpGet]
        [SwaggerResponse(200)]
        [SwaggerResponse(401, Description = "Unauthorized. The credentials provided failed authentication.")]
        public async Task<string> Get()
        {
            // prep claims (since we need to add the "sub" claim for JWT)
            var claimsPrincipal = new ClaimsPrincipal(User.Identity);
            var claims = new List<Claim>();
            claims.AddRange(claimsPrincipal.Claims);

            // the subject is the Client ID
            claims.Add(new Claim(
                "sub",
                claimsPrincipal.FindFirst(
                    x => x.Type == CustomClaimTypes.ClientId).Value));

            var token = new JwtSecurityToken(
                issuer: JwtBearerConstants.TokenIssuer,
                audience: JwtBearerConstants.TokenAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: null,
                signingCredentials: new SigningCredentials(
                    Crypto.GenerateSigningKey(_config.Secret),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
