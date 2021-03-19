using Microsoft.IdentityModel.Tokens;

using System;

namespace Biobanks.Submissions.Api.Auth
{
    /// <summary>
    /// Cryptography helpers for Identity purposes
    /// </summary>
    public static class Crypto
    {
        /// <summary>
        /// Generate a Signing Key from a Secret, that can be used to sign / verify JWTs
        /// </summary>
        /// <param name="secret"></param>
        public static SymmetricSecurityKey GenerateSigningKey(string secret)
            => new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(secret));
        
        /// <summary>
        /// Generate a random minimum 128bit strong Secret of suitable entropy
        /// </summary>
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
