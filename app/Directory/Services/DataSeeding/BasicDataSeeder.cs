using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Services.DataSeeding;

public class BasicDataSeeder
{
  private readonly ILogger<BasicDataSeeder> _logger;
  private readonly ApplicationDbContext _db;

  public BasicDataSeeder(
    ILoggerFactory logger,
    ApplicationDbContext db)
  {
    _logger = logger.CreateLogger<BasicDataSeeder>();
    _db = db;
  }

  /// <summary>
  /// Read a JSON file
  /// and seed its contents into a table of Entity Type `T`.
  ///
  /// If a directory path is provided, the JSON filename will match the typename
  /// </summary>
  /// <typeparam name="T">The EF Entity Type</typeparam>
  public async Task SeedJson<T>(string path) where T : class => await SeedEmpty(Seeding.ReadJson<T>(path));

  /// <summary>
  /// Generic seed method to handle most cases of seeding multiple entities into an empty table.
  /// </summary>
  /// <param name="entities">The entities to be seeded</param>
  /// <typeparam name="T">The EF Entity Type</typeparam>
  public async Task SeedEmpty<T>(IEnumerable<T> entities) where T : class
  {
    var set = _db.Set<T>();

    // only seed empty tables <3
    var count = set.Count();
    if (count > 0)
    {
      _logger.LogInformation("{type}: {count} entries already exist", typeof(T).Name, count);
      return;
    }

    var entityList = entities.ToList(); // avoid multiple enumerations when we get counts
    await set.AddRangeAsync(entityList);

    /*
     * Tables with auto increment id's need their sequences fixing
     *
     * Note we naively assume only one autoincrement column in a table,
     * with a unique sequence for only itself!
     *
     * Note we naively use the count to do this
     * So id's in seed data should be contiguous from 1
     * and not exceed the number of records being seeded
     * otherwise future inserts will fail
     */
    var props = set.EntityType.GetProperties();
    if (props.Any(x => x.ValueGenerated == ValueGenerated.OnAdd)) // any autoincrement columns?
    {
      var tableName = set.EntityType.GetTableName();

      // TODO: handle multiple columns?
      var columnName = set.EntityType.GetValueGeneratingProperties().First().Name;

      await using var transaction = await _db.Database.BeginTransactionAsync();

      await _db.SaveChangesAsync();

      // TODO: figure out how to do this by max id instead of count :/
      await _db.Database.ExecuteSqlRawAsync(
        // This is the postgres way; this will differ for different db providers
        $"ALTER SEQUENCE \"{tableName}_{columnName}_seq\" RESTART WITH {entityList.Count + 1}");

      await transaction.CommitAsync();
    }
    else
    {
      await _db.SaveChangesAsync();
    }

    _logger.LogInformation("{type}: Written {count} entries", typeof(T).Name, entityList.Count);
  }
}
