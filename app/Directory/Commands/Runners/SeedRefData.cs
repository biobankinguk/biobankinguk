using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Api.ReferenceData;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Commands.Helpers;
using Biobanks.Directory.Services.DataSeeding;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands.Runners;

internal class SeedRefData
{
  private readonly ILogger<SeedRefData> _logger;
  private readonly IConsole _console;
  private readonly BasicDataSeeder _seeder;
  private readonly CustomRefDataSeeder _customSeeder;
  private readonly FixedRefDataSeeder _fixedSeeder;

  private string _dataDirectory;
  private readonly List<Func<Task>> _seedActions;

  private Task SeedJson<T>() where T : class
    => _seeder.SeedJson<T>(_dataDirectory);

  public SeedRefData(
    ILoggerFactory logger,
    IConsole console,
    BasicDataSeeder seeder,
    CustomRefDataSeeder customSeeder,
    FixedRefDataSeeder fixedSeeder)
  {
    _logger = logger.CreateLogger<SeedRefData>();
    _console = console;
    _seeder = seeder;
    _customSeeder = customSeeder;
    _fixedSeeder = fixedSeeder;

    // Setup Seed Actions
    // List Order determines Seed Order!
    _seedActions = new()
    {
      /* Fixed Data */
      fixedSeeder.Seed,
      
      /* Directory Specific */
      SeedCountries,
      SeedJson<AccessCondition>,
      SeedJson<AgeRange>,
      customSeeder.SeedAnnualStatistics,
      SeedJson<AssociatedDataProcurementTimeframe>,
      customSeeder.SeedAssociatedDataTypes,
      SeedJson<CollectionPercentage>,
      SeedJson<CollectionStatus>,
      SeedJson<CollectionType>,
      SeedJson<ConsentRestriction>,
      SeedJson<DonorCount>,
      SeedJson<Funder>,
      SeedJson<MacroscopicAssessment>,
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
      customSeeder.SeedMaterialTypes,
      SeedJson<StorageTemperature>,
      customSeeder.SeedPreservationTypes
    };
  }

  private Task SeedCountries()
  {
    var fetchRemote = new Prompt(_console).YesNo("Would you like to seed UN Countries?");
    return _customSeeder.SeedCountries(fetchRemote);
  }

  public async Task Run(string dataDirectory)
  {
    _customSeeder.DataDirectory = _dataDirectory = dataDirectory;

    foreach (var seed in _seedActions) await seed();

    _logger.LogInformation("Seeding Complete - Ran {count} Seeding Actions", _seedActions.Count);
  }
}
