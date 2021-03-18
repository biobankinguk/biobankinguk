using static IdentityModel.CryptoRandom;
using CryptoRandom = IdentityModel.CryptoRandom;


namespace Biobanks.IdentityModel.Services
{
    public class CryptoService
    {
        /// <summary>
        /// <para>Create a Unique ID as a series of bytes, then convert it to a string in the specified format.</para>
        /// <para>
        /// If the Unique ID is of sufficient length, it can be considered a strong secure ID, suitable for use a hashed secret.
        /// e.g. a length of 24 will give a 128bit strong value.
        /// </para>
        /// </summary>
        /// <param name="length">The length</param>
        /// <param name="format">The output format</param>
        public static string CreateUniqueId(int length = 32, OutputFormat format = OutputFormat.Base64Url)
            // Today this is simply a wrapper around IdentityModel's CryptoRandom.CreateUniqueId
            // but it ensures consistent usage throughout the codebase
            // and eases a change of implementation later.
            => CryptoRandom.CreateUniqueId(length, format);
    }
}
