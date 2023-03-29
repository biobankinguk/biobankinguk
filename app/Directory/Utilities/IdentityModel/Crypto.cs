using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Biobanks.Directory.Utilities.IdentityModel
{
  public static class Crypto
  {
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    /// <summary>
    /// Generates a cryptographically random byte array
    /// </summary>
    /// <param name="length">The desired length of the byte array</param>
    public static byte[] GenerateRandomBytes(int length)
    {
      var bytes = new byte[length];
      Rng.GetBytes(bytes);
      return bytes;
    }

    /// <summary>
    /// <para>Generate a Unique ID as a string in the specified format.</para>
    /// <para>
    /// If the Unique ID is of sufficient byte length, it can be considered a strong secure ID, suitable for use a hashed secret.
    /// e.g. a length of 32 will give a 256bit strong value.
    /// </para>
    /// </summary>
    /// <param name="length">The length</param>
    /// <param name="format">The output format</param>
    public static string GenerateId(int length = 32, OutputFormat format = OutputFormat.Base64Url)
    {
      var id = GenerateRandomBytes(length);

      return format switch
      {
        OutputFormat.Base64Url => Base64UrlEncoder.Encode(id),
        OutputFormat.Base64 => Convert.ToBase64String(id),
        OutputFormat.Hex => BitConverter.ToString(id).Replace("-", ""),
        _ => throw new ArgumentException("Invalid OutputFormat", nameof(format))
      };
    }

    /// <summary>
    /// Generate a Signing Key from a Base64Url Secret, that can be used to sign / verify JWTs
    /// </summary>
    /// <param name="secret">The secret to use for the key, in Base64Url format</param>
    public static SymmetricSecurityKey GenerateSigningKey(string secret)
      => new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(secret));
  }
}
