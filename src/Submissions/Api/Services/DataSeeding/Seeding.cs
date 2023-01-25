using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Services.DataSeeding;

/// <summary>
/// Static Helper methods for seeding
/// </summary>
public static class Seeding
{
  /// <summary>
  /// Deserialize JSON from a file to a List of Entities.
  /// </summary>
  /// <param name="path">
  /// <para>Either the direct path to a source JSON file,
  /// or the path to a directory in which a source JSON file lives.
  /// </para>
  /// <para>
  /// If the path is to a directory, the filename is inferred to be `[EntityType].json`
  /// </para></param>
  /// <typeparam name="T">Target EF Entity Type</typeparam>
  /// <returns></returns>
  public static ICollection<T> ReadJson<T>(string path)
  {
    // TODO: Consider switching from Newtonsoft to STJ
    
    // Infer filename if the path is a directory
    if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
      path = Path.Combine(path, $"{typeof(T).Name}.json");

    using var stream = new StreamReader(path);
    using var reader = new JsonTextReader(stream);

    return new JsonSerializer().Deserialize<ICollection<T>>(reader);
  }
}
