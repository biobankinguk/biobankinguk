using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Data.Transforms.Url;
public static class UrlTransformer
{
  public static string Transform(string urlString)
  {
    //no url is acceptable - it's not mandatory
    if (string.IsNullOrEmpty(urlString)) return string.Empty;

    var validSchemes = new[]
    {
                UrlSchemes.Http,
                UrlSchemes.Https
            };

    Uri url;

    //Try and make a relative URL from the string (i.e. assume they've left the scheme off)
    try
    {
      url = new Uri(urlString, UriKind.Relative);

      //successful relative URL? make it into an absolute one by adding a default scheme
      urlString = UrlSchemes.Default.AsUrlPrefix() + url.OriginalString;
    }
    catch (UriFormatException) { }

    //Try and make an absolute URL (if the relative one failed, or now that we've added the scheme
    url = new Uri(urlString, UriKind.Absolute); //this will throw if the format is still invalid, and that's fine

    //check for a mistyped scheme (e.g. http//)
    //(which will appear as the hostname, with a path starting with double slash
    var hostScheme = validSchemes.ToList().FirstOrDefault(s => url.Host == s);
    if (hostScheme != null)
    {
      //abandon the default scheme we added, and use the one they mistyped
      url = new Uri($"{url.Host}:{url.PathAndQuery}", UriKind.Absolute);
    }

    //validate the scheme
    if (!validSchemes.Contains(url.Scheme))
    {
      throw new InvalidUrlSchemeException($"{url.Scheme} is not a valid URL scheme. Please use the http:// or https:// scheme.");
    }

    return url.AbsoluteUri; // return the URI version, in case it improved the form of the original string
  }
}

