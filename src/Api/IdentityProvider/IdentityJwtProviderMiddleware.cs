using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Biobanks.IdentityProvider
{
    public class IdentityJwtProviderMiddleware
    {
        private readonly IConfiguration _configuration;

        public IdentityJwtProviderMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                await context.ChallengeAsync();

            if (context.Response.StatusCode != 200) return;

            //if we're here, we must have an authenticated identity, and can therefore generate a jwt

            //prep claims (since we need to add the "sub" claim for JWT)
            var claims = new List<Claim>();
            claims.AddRange(new ClaimsPrincipal(context.User.Identity).Claims);
            claims.Add(new Claim("sub", context.User.Identity.Name));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: null,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Convert.FromBase64String(
                            // Secret should be a Base64 string for easy byte encoding
                            // It also needs to have a minimum length of
                            // 24 (encoded) characters
                            // to ensure the Security Key is at least 128bit strong
                            _configuration["JWT:Secret"])),
                    SecurityAlgorithms.HmacSha256)
            );

            var handler = new JwtSecurityTokenHandler();

            await context.Response.WriteAsync(handler.WriteToken(token));
        }
    }
}
