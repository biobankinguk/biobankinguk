using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobanks.Directory.Services.DataSeeding;

internal class CountriesDto
{
  [JsonProperty("isCountry")] public bool IsCountry { get; set; }

  [JsonProperty("countryName")] public string CountryName { get; set; }
}

internal class CustomRefDataSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly BasicDataSeeder _seeder;
  private readonly HttpClient _countriesApi;

  public string DataDirectory { get; set; } = "./data";

  public CustomRefDataSeeder(
    IConfiguration config,
    IHttpClientFactory httpClient,
    ApplicationDbContext db,
    BasicDataSeeder seeder)
  {
    _db = db;
    _seeder = seeder;

    _countriesApi = httpClient.CreateClient();
    _countriesApi.BaseAddress = new Uri(config["CountriesApiUrl"] ?? "/");
  }

  public async Task SeedAnnualStatistics()
  {
    var groups = Seeding.ReadJson<AnnualStatisticGroup>(DataDirectory);

    // Seed AnnualStatisticGroups
    await _seeder.SeedEmpty(groups.Select(x =>
      new AnnualStatisticGroup
      {
        Id = x.Id,
        Value = x.Value.Trim()
      }
    ));

    // Seed AnnualStatistics
    await _seeder.SeedEmpty(groups.SelectMany(x =>
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

  public async Task SeedAssociatedDataTypes()
  {
    var groups = Seeding.ReadJson<AssociatedDataTypeGroup>(DataDirectory);

    // Seed AssociatedDataTypeGroups
    await _seeder.SeedEmpty(groups.Select(x =>
      new AssociatedDataTypeGroup
      {
        Id = x.Id,
        Value = x.Value.Trim()
      }
    ));

    // Seed AssociatedDataTypes
    await _seeder.SeedEmpty(groups.SelectMany(x =>
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

  public async Task SeedCountries(bool fetchRemote = false)
  {
    if (fetchRemote)
    {
      var countries = await _countriesApi.GetFromJsonAsync<List<CountriesDto>>("");
      await _seeder.SeedEmpty(countries.Select(x => new Country { Value = x.CountryName }));
    }
    else
    {
      var countries = Seeding.ReadJson<Country>(DataDirectory);

      // Seed Countries
      await _seeder.SeedEmpty(countries.Select(x =>
        new Country
        {
          Id = x.Id,
          Value = x.Value.Trim()
        }));

      // Seed Counties
      await _seeder.SeedEmpty(countries.SelectMany(x =>
        x.Counties.Select(y =>
          new County
          {
            Id = y.Id,
            Value = y.Value.Trim(),
            CountryId = x.Id
          })));
    }

    // Update Config Value
    var configKey = "site.display.counties";
    var config = _db.Configs.Local // EF might already be tracking this config if we seeded it earlier
                   .SingleOrDefault(x => x.Key == configKey)
                 ?? new Data.Entities.Config()
                 {
                   Key = configKey
                 };

    config.Value = !fetchRemote ? "true" : "false";

    if (_db.Configs.Any(x => x.Key == config.Key))
      _db.Configs.Update(config);
    else
      _db.Configs.Add(config);
    
    await _db.SaveChangesAsync();
  }

  public async Task SeedMaterialTypes()
  {
    var validGroups = _db.MaterialTypeGroups.ToList();
    var validProcedures = _db.OntologyTerms.ToList();

    await _seeder.SeedEmpty(
      Seeding.ReadJson<MaterialType>(DataDirectory)
        .Select(x =>
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

  public async Task SeedPreservationTypes()
  {
    var validTemperatures = _db.StorageTemperatures.ToList();

    await _seeder.SeedEmpty(
      Seeding.ReadJson<PreservationType>(DataDirectory)
        .Select(x =>
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
}
