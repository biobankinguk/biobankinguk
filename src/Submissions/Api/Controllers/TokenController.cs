using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Config;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

        [HttpGet]
        // TODO: Swaggger annotations
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
