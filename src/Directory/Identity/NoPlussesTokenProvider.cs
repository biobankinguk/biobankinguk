using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Directory.Identity
{
    
    /// <inheritdoc />
    public class NoPlussesTokenProvider<TUser> : NoPlussesTokenProvider<TUser, string>
        where TUser : class, IUser<string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NoPlussesTokenProvider(IDataProtector protector) : base(protector)
        {
        }
    }

    /// <summary>
    /// Token provider that uses an IDataProtector to generate encrypted tokens based off of the security stamp.
    /// Returns a base64 string except '+' is replaced with '--'. 
    /// This is used because the NHS has a faulty link validator which doesn't double-encode '+' into %252B in its querystring.
    /// </summary>
    public class NoPlussesTokenProvider<TUser, TKey> : IUserTokenProvider<TUser, TKey>
        where TUser : class, IUser<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NoPlussesTokenProvider(IDataProtector protector)
        {
            Protector = protector ?? throw new ArgumentNullException("protector");
            TokenLifespan = TimeSpan.FromDays(1);
        }

        public IDataProtector Protector { get; private set; }

        public TimeSpan TokenLifespan { get; set; }

        /// <summary>
        /// Generates a new token for given purpose and user.
        /// </summary>
        public async Task<string> GenerateAsync(string purpose, UserManager<TUser, TKey> manager, TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            
            var ms = new MemoryStream();
            using (var writer = ms.CreateWriter())
            {
                writer.Write(DateTimeOffset.UtcNow);
                writer.Write(Convert.ToString(user.Id, CultureInfo.InvariantCulture));
                writer.Write(purpose ?? string.Empty);

                string stamp = null;
                if (manager.SupportsUserSecurityStamp)
                    stamp = await manager.GetSecurityStampAsync(user.Id).WithCurrentCulture();
                
                writer.Write(stamp ?? string.Empty);
            }
            var protectedBytes = Protector.Protect(ms.ToArray());
            return Convert.ToBase64String(protectedBytes).Replace("+", "--");
        }

        /// <summary>
        /// Return false if the token is not valid
        /// </summary>
        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, TKey> manager, TUser user)
        {
            try
            {
                token = token.Replace("--", "+");
                var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
                var ms = new MemoryStream(unprotectedData);
                using (var reader = ms.CreateReader())
                {
                    var creationTime = reader.ReadDateTimeOffset();
                    var expirationTime = creationTime + TokenLifespan;
                    if (expirationTime < DateTimeOffset.UtcNow)
                        return false;

                    var userId = reader.ReadString();
                    if (!string.Equals(userId, Convert.ToString(user.Id, CultureInfo.InvariantCulture)))
                        return false;

                    var purp = reader.ReadString();
                    if (!string.Equals(purp, purpose))
                        return false;
                    
                    var stamp = reader.ReadString();
                    if (reader.PeekChar() != -1)
                        return false;

                    if (manager.SupportsUserSecurityStamp)
                    {
                        var expectedStamp = await manager.GetSecurityStampAsync(user.Id).WithCurrentCulture();
                        return stamp == expectedStamp;
                    }
                    return stamp == string.Empty;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Do not leak exception
            }
            return false;
        }

        /// <summary>
        /// Returns true if the provider can be used to generate tokens for this user. Always true.
        /// </summary>
        public Task<bool> IsValidProviderForUserAsync(UserManager<TUser, TKey> manager, TUser user)
            => Task.FromResult(true);

        /// <summary>
        /// This provider no-ops by default when asked to notify a user.
        /// </summary>
        public Task NotifyAsync(string token, UserManager<TUser, TKey> manager, TUser user)
            => Task.FromResult(0);
    }

    // Lifted from source of Microsoft.AspNet.Identity.Owin/DataProtectorTokenProvider
    internal static class StreamExtensions
    {
        internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        public static BinaryReader CreateReader(this Stream stream) 
            => new BinaryReader(stream, DefaultEncoding, true);

        public static BinaryWriter CreateWriter(this Stream stream) 
            => new BinaryWriter(stream, DefaultEncoding, true);

        public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader) 
            => new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);

        public static void Write(this BinaryWriter writer, DateTimeOffset value)
            => writer.Write(value.UtcTicks);
    }
}
