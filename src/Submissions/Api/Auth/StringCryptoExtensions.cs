using System;
using System.Security.Cryptography;
using System.Text;

namespace Biobanks.Submissions.Api.Auth
{
    public static class StringCryptoExtensions
    {
        public static string Sha256(this string input)
        {

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(sha256.ComputeHash(bytes));
            }
        }
    }
}
