using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Auth.Basic
{
    /// <summary>
    /// AuthenticationScheme Options for Basic Auth
    /// </summary>
    public class BasicAuthSchemeOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Authentication Scheme Realm - required by Basic Auth
        /// </summary>
        public string Realm { get; set; }
    }

    /// <summary>
    /// This is used to make Realm above mandatory as per the Basic Authentication spec https://tools.ietf.org/html/rfc7617
    /// </summary>
    public class BasicAuthPostConfigureOptions : IPostConfigureOptions<BasicAuthSchemeOptions>
    {
        /// <summary>
        /// Runs after BasicAuthSchemeOptions Configuration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void PostConfigure(string name, BasicAuthSchemeOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }
}
