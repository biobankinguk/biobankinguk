using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using System;

namespace Biobanks.Submissions.Api.Auth.Basic
{
    public class BasicAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }

    /// <summary>
    /// This is used to make Realm above mandatory as per the Basic Authentication spec https://tools.ietf.org/html/rfc7617
    /// </summary>
    public class BasicAuthPostConfigureOptions : IPostConfigureOptions<BasicAuthSchemeOptions>
    {
        public void PostConfigure(string name, BasicAuthSchemeOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }
}
