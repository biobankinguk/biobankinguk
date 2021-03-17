using Microsoft.IdentityModel.Tokens;

using System.Security.Cryptography;
using System.Text;

namespace Biobanks.Submissions.Api.Auth
{
    public static class StringCryptoExtensions
    {
        /// <summary>
        /// Hash a string using SHA256 and Base64 URL encode it
        /// </summary>
        /// <param name="input"></param>
        public static string Sha256(this string input)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Base64UrlEncoder.Encode(sha256.ComputeHash(bytes));
        }
    }
}
