using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace Directory.IdentityServer
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="client_id">The client identifier.</param>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (string.IsNullOrWhiteSpace(client_id)) return false;

            return (await store.FindEnabledClientByIdAsync(client_id))?
                .RequirePkce ?? false;
        }
    }
}
