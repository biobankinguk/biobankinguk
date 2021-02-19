using System;
using System.Linq;

namespace Biobanks.DataSeed.Transforms
{
    public static class UrlTransformer
    {
        public static bool TryValidateUrl(string urlString, out string validatedUrl)
        {
            var validSchemes = new[]
            {
                Uri.UriSchemeHttp,
                Uri.UriSchemeHttps
            };

            // Null By Default
            validatedUrl = null;

            //Try and make a relative URL from the string (i.e. assume they've left the scheme off)
            if (Uri.TryCreate(urlString, UriKind.Relative, out var relUrl))
            {
                urlString = Uri.UriSchemeHttp + relUrl.OriginalString;
            }

            if (!Uri.TryCreate(urlString, UriKind.Absolute, out var url))
            {
                return false;
            }

            // Check For Mistyped Scheme -> Manual Parsing
            if (validSchemes.Any(s => url.Host == s))
            {
                url = new Uri($"{url.Host}:{url.PathAndQuery}", UriKind.Absolute);
            }

            // Validate Scheme
            if (validSchemes.Contains(url.Scheme))
            {
                validatedUrl = url.AbsoluteUri;
            }

            return !string.IsNullOrEmpty(validatedUrl);
        }
    }
}
