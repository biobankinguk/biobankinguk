using System.Collections.Generic;
using IdentityServer4.Models;

namespace Directory.IdentityServer
{
    /// <summary>
    /// Purely a configuration class.
    /// These IdentityResources are seeded into the config db
    /// </summary>
    public static class IdentityResourceData
    {
        /// <summary>
        /// All the proconfigured IdentityResources
        /// </summary>
        public static IEnumerable<IdentityResource> List()
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
                // Add other / custom IdentityResources here
                // e.g. new IdentityResource("my-resource", "My Resource", new Claim(CustomClaimTypes.MyResource, "value"))
            };
    }
}
