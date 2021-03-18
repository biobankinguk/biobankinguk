using Biobanks.IdentityModel.Types;

using Microsoft.IdentityModel.Tokens;

using System;
using System.Security.Cryptography;


namespace Biobanks.IdentityModel.Services
{
    public class CryptoService
    {
        private readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Gets a cryptographically random byte array
        /// </summary>
        /// <param name="length">The desired length of the byte array</param>
        public byte[] GetRandomBytes(int length)
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
        public string GenerateId(int length = 32, OutputFormat format = OutputFormat.Base64Url)
        {
            var id = GetRandomBytes(length);

            return format switch
            {
                OutputFormat.Base64Url => Base64UrlEncoder.Encode(id),
                OutputFormat.Base64 => Convert.ToBase64String(id),
                OutputFormat.Hex => BitConverter.ToString(id).Replace("-", ""),
                _ => throw new ArgumentException("Invalid OutputFormat", nameof(format))
            };
        }
    }
}
