using Biobanks.Submissions.Api.Constants;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace Biobanks.Submissions.Api.Auth;

public static class StringEncodingExtensions
{
  public static string Utf8ToBase64Url(this string input)
    => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(input));

  public static object ObjectToBase64UrlJson(this object input)
  => JsonSerializer.Serialize(input, DefaultJsonOptions.Serializer).Utf8ToBase64Url();
}
