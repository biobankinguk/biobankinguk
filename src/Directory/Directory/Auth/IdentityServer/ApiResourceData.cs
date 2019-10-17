using System.Collections.Generic;
using Common.Constants;
using IdentityServer4.Models;

namespace Directory.Auth.IdentityServer
{
    /// <summary>
    /// Purely a configuration class.
    /// These ApiResources are seeded into the config db
    /// </summary>
    public static class ApiResourceData
    {
        /// <summary>
        /// All the preconfigured ApiResources
        /// </summary>
        public static IEnumerable<ApiResource> List()
            => new ApiResource[]
            {
                new ApiResource(ApiResourceKeys.RefData, "UKCRC TDCC Reference Data API"),
                new ApiResource(ApiResourceKeys.Upload, "UKCRC TDCC Upload API")
                // Add more ApiResources here
            };
    }
}
