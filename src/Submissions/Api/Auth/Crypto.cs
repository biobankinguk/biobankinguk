using Microsoft.IdentityModel.Tokens;

using System;

namespace Biobanks.Submissions.Api.Auth
{
    public static class Crypto
    {
        public static SymmetricSecurityKey GenerateSigningKey(string secret)
            => new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(secret));

        public static string GenerateSecret()
        {
            // Secret should be a Base64 string for easy byte encoding
            // It also needs to have a minimum length of
            // 24 (encoded) characters
            // to ensure the Security Key is at least 128bit strong

            // TODO: use IdentityModel.CryptoRandom
            throw new NotImplementedException();
        }
    }
}
