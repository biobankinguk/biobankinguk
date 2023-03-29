using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Biobanks.Directory.Utilities.IdentityModel
{
  /// <summary>
  /// Cryptography related Extension Methods for string
  /// </summary>
  public static class StringCryptoExtensions
  {
    /// <summary>
    /// Hash a UTF8 string using SHA256 and Base64URL encode it
    /// </summary>
    /// <param name="input">The UT8 string to hash</param>
    public static string Sha256(this string input)
    {
      using SHA256 sha256 = SHA256.Create();
      byte[] bytes = Encoding.UTF8.GetBytes(input);
      return Base64UrlEncoder.Encode(sha256.ComputeHash(bytes));
    }
  }
}
