using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Data.Transforms.Url;
public static class UrlSchemes
{
  public static string Default => Http;

  public static string Http => "http";
  public static string Https => "https";

  public static string AsPrefix(string scheme) => $"{scheme}://";
  public static string AsUrlPrefix(this string scheme) => $"{scheme}://"; //extension method version :)
}
