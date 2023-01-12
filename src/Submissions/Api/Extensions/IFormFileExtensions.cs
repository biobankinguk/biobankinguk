using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace Biobanks.Submissions.Api.Extensions;

public static class IFormFileExtensions
{
  public static Stream ToProcessableStream(this IFormFile file)
  {
    if (file.Length > 0)
      return file.OpenReadStream();

    throw new FileLoadException("The supplied posted file does not have any content.");
  }

  /// <summary>
  /// Returns true if a logo suitable for uploading, false if not, throws exceptions in the case of validation errors.
  /// </summary>
  /// <param name="file">The uploaded logo file</param>
  /// <returns>True if there is a valid logo to upload, False if not</returns>
  public static bool ValidateAsLogo(this IFormFile file)
  {
    if (file == null || file.Length <= 0) return false; //nothing to validate or upload

    if (file.Length > 1000000)
    {
      throw new BadImageFormatException("Images must be less than 1Mb in size.");
    }

    //validate format (ContentType)
    if (!new[]
    {
                "image/gif", "image/jpeg", "image/pjpeg", "image/png"
            }.Contains(file.ContentType))
    {
      throw new BadImageFormatException("Logo must be a valid GIF, JPEG or PNG file.");
    }

    return true;
  }
}
