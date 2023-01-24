using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Commands.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Commands.Runners;

internal class CountriesDto
{
  [JsonProperty("isCountry")] public bool IsCountry { get; set; }

  [JsonProperty("countryName")] public string CountryName { get; set; }
}

internal class SeedRefData
{
  private readonly ILogger<SeedRefData> _logger;
  private readonly IConsole _console;
  private readonly ApplicationDbContext _db;
  private readonly HttpClient _countriesApi;

  private string _dataDirectory = "./data";


  private readonly List<Func<Task>> _seedActions;

  public SeedRefData(
    ILoggerFactory logger,
    IConsole console,
    IConfiguration config,
    IHttpClientFactory httpClient,
    ApplicationDbContext db)
  {
    _logger = logger.CreateLogger<SeedRefData>();
    _console = console;
    _db = db;

    _countriesApi = httpClient.CreateClient();
    _countriesApi.BaseAddress = new Uri(config["CountriesApiKey"]);

    // Setup Seed Actions
    // List Order determines Seed Order!
    _seedActions = new()
    {
      /* Directory Specific */
      SeedCountries,
      SeedJson<AccessCondition>,
      SeedJson<AgeRange>,
      SeedAnnualStatistics,
      SeedJson<AssociatedDataProcurementTimeframe>,
      SeedAssociatedDataTypes,
      SeedJson<CollectionPercentage>,
      SeedJson<CollectionStatus>,
      SeedJson<CollectionType>,
      SeedJson<ConsentRestriction>,
      SeedJson<DonorCount>,
      SeedJson<Funder>,
      SeedJson<MacroscopicAssessment>,
      SeedOrganisationTypes,
      SeedJson<RegistrationReason>,
      SeedJson<SampleCollectionMode>,
      SeedJson<ServiceOffering>,
      SeedJson<SopStatus>,

      /* API Specific */
      SeedJson<Ontology>,
      SeedJson<SampleContentMethod>,
      SeedJson<Status>,
      SeedJson<TreatmentLocation>,

      /* Shared */
      SeedJson<Sex>,
      SeedJson<SnomedTag>,
      SeedJson<OntologyTerm>,
      SeedJson<MaterialTypeGroup>,
      SeedMaterialTypes,
      SeedJson<StorageTemperature>,
      SeedPreservationTypes
    };
  }

  public async Task Run(string dataDirectory)
  {
    _dataDirectory = dataDirectory;

    foreach (var seed in _seedActions) await seed();
    _logger.LogInformation("Seeding Complete - Ran {count} Seeding Actions", _seedActions.Count);
  }

  private async Task SeedAnnualStatistics()
  {
    var groups = ReadJson<AnnualStatisticGroup>();

    // Seed AnnualStatisticGroups
    await Seed(groups.Select(x =>
      new AnnualStatisticGroup
      {
        Id = x.Id,
        Value = x.Value.Trim()
      }
    ));

    // Seed AnnualStatistics
    await Seed(groups.SelectMany(x =>
      x.AnnualStatistics.Select(y =>
        new AnnualStatistic
        {
          Id = y.Id,
          Value = y.Value.Trim(),
          AnnualStatisticGroupId = x.Id
        }
      )
    ));
  }

  private async Task SeedAssociatedDataTypes()
  {
    var groups = ReadJson<AssociatedDataTypeGroup>();

    // Seed AssociatedDataTypeGroups
    await Seed(groups.Select(x =>
      new AssociatedDataTypeGroup
      {
        Id = x.Id,
        Value = x.Value.Trim()
      }
    ));

    // Seed AssociatedDataTypes
    await Seed(groups.SelectMany(x =>
      x.AssociatedDataTypes.Select(y =>
        new AssociatedDataType
        {
          Id = y.Id,
          Value = y.Value.Trim(),
          AssociatedDataTypeGroupId = x.Id
        }
      )
    ));
  }

  private async Task SeedCountries()
  {
    var fetchRemote = new Prompt(_console).YesNo("Would you like to seed UN Countries?");

    if (fetchRemote)
    {
      var countries = await _countriesApi.GetFromJsonAsync<List<CountriesDto>>("");
      await Seed(countries.Select(x => new Country { Value = x.CountryName }));
    }
    else
    {
      var countries = ReadJson<Country>();

      // Seed Countries
      await Seed(countries.Select(x =>
        new Country
        {
          Id = x.Id,
          Value = x.Value.Trim()
        }));

      // Seed Counties
      await Seed(countries.SelectMany(x =>
        x.Counties.Select(y =>
          new County
          {
            Id = y.Id,
            Value = y.Value.Trim(),
            CountryId = x.Id
          })));
    }

    // Update Config Value
    var config = new Entities.Data.Config()
    {
      Key = "site.display.counties",
      Value = !fetchRemote ? "true" : "false"
    };

    if (_db.Configs.Any(x => x.Key == config.Key))
    {
      _db.Configs.Update(config);
    }
    else
    {
      _db.Configs.Add(config);
    }

    await _db.SaveChangesAsync();
  }

  private async Task SeedMaterialTypes()
  {
    var validGroups = _db.MaterialTypeGroups.ToList();
    var validProcedures = _db.OntologyTerms.ToList();

    await Seed(
      ReadJson<MaterialType>().Select(x =>
        new MaterialType()
        {
          Id = x.Id,
          Value = x.Value.Trim(),
          SortOrder = x.SortOrder,
          MaterialTypeGroups =
            x.MaterialTypeGroups?
              .Select(y => validGroups.First(z => z.Value == y.Value.Trim()))
              .ToList(),
          ExtractionProcedures =
            x.ExtractionProcedures?
              .Select(y => validProcedures.First(z => z.Id == y.Id.Trim()))
              .ToList()
        }
      )
    );
  }

  private async Task SeedOrganisationTypes()
    => await Seed(new List<OrganisationType>
    {
      new()
      {
        OrganisationTypeId = 1,
        Description = "Biobank",
        SortOrder = 1
      }
    });

  private async Task SeedPreservationTypes()
  {
    var validTemperatures = _db.StorageTemperatures.ToList();

    await Seed(
      ReadJson<PreservationType>().Select(x =>
        new PreservationType
        {
          Id = x.Id,
          Value = x.Value.Trim(),
          SortOrder = x.SortOrder,
          StorageTemperature = validTemperatures.First(y => y.Value == x.StorageTemperature.Value)
        }
      )
    );
  }

  /// <summary>
  /// Read a JSON file from the data directory
  /// and seed its contents into a table of Entity Type `T`.
  ///
  /// The JSON filename will match the typename.
  /// </summary>
  /// <typeparam name="T">The EF Entity Type</typeparam>
  private async Task SeedJson<T>() where T : class => await Seed(ReadJson<T>());

  /// <summary>
  /// Generic seed method to handle most cases of seeding multiple entities into a table.
  /// </summary>
  /// <param name="entities">The entities to be seeded</param>
  /// <typeparam name="T">The EF Entity Type</typeparam>
  private async Task Seed<T>(IEnumerable<T> entities) where T : class
  {
    var set = _db.Set<T>();

    // only seed empty tables <3
    var count = set.Count();
    if (count > 0)
    {
      _logger.LogInformation("{type}: {count} entries already exist", typeof(T).Name, count);
      return;
    }

    var list = entities.ToList();
    await set.AddRangeAsync(list);

    // TODO: fix auto increment scenarios for postgres
    // var table = set.EntityType.GetTableName();
    // var props = set.EntityType.GetProperties();

    // Save Changes
    // Check If Table Has Identity Column, Hence Needing Identity Insert
    // if (identityInsert && props.Any(x => x.ValueGenerated == ValueGenerated.OnAdd))
    // {
    //   using var transaction = _db.Database.BeginTransaction();
    //
    //   _db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{table} ON");
    //   _db.SaveChanges();
    //   _db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{table} OFF");
    //                 
    //   transaction.Commit();
    // }
    // else
    // {
    await _db.SaveChangesAsync();
    // }

    _logger.LogInformation("{type}: Written {count} entries", typeof(T).Name, list.Count);
  }

  /// <summary>
  /// Deserialize JSON from a file to a List of Entities.
  /// </summary>
  /// <param name="filePath">Override JSON filepath.
  /// The default is to match the Entity Type name in the data directory</param>
  /// <typeparam name="T">Target EF Entity Type</typeparam>
  /// <returns></returns>
  private ICollection<T> ReadJson<T>(string filePath = "")
  {
    // TODO: Consider switching from Newtonsoft to STJ

    if (string.IsNullOrWhiteSpace(filePath))
      filePath = Path.Combine(_dataDirectory, $"{typeof(T).Name}.json");

    using var stream = new StreamReader(filePath);
    using var reader = new JsonTextReader(stream);

    return new JsonSerializer().Deserialize<ICollection<T>>(reader);
  }
}
