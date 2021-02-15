using Biobanks.Data;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Biobanks.DataSeed.Services
{
    internal class SeedingService : IHostedService
    {
        private const string _dataDir = "data";

        private readonly ILogger<SeedingService> _logger;
        private readonly BiobanksDbContext _db;
        private readonly CountriesWebService _countriesWebService;

        private readonly IEnumerable<Action> _seedActions;

        public SeedingService(ILogger<SeedingService> logger, BiobanksDbContext db, CountriesWebService countriesWebService)
        {
            _db = db;
            _logger = logger;
            _countriesWebService = countriesWebService;

            // List Order Determines Seed Order
            _seedActions = new List<Action>
            {
                /* Directory Specific */
                SeedCountries,
                SeedJson<AccessCondition>,
                SeedJson<AgeRange>,
                SeedJson<AnnualStatisticGroup>,
                SeedJson<AssociatedDataProcurementTimeframe>,
                SeedJson<AssociatedDataTypeGroup>,
                SeedJson<AssociatedDataType>,
                SeedJson<CollectionPercentage>,
                SeedJson<CollectionPoint>,
                SeedJson<CollectionStatus>,
                SeedJson<CollectionType>,
                SeedJson<ConsentRestriction>,
                SeedJson<DonorCount>,
                SeedJson<Funder>,
                SeedJson<HtaStatus>,
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
                SeedJson<MaterialTypeGroup>,
                SeedMaterialTypes,
                SeedJson<Sex>,
                SeedJson<OntologyTerm>,
                SeedJson<StorageTemperature>,
                SeedJson<PreservationType>
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var seedAction in _seedActions)
            {
                seedAction();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        private void SeedCountries()
        {
            var seedUN = Prompt.GetYesNo("Would you like to seed UN Countries?", false);

            // Seed Countries
            if (seedUN)
            {
                Seed(
                    _countriesWebService.ListCountriesAsync().Result.Select(x => 
                        new Country
                        {
                            Value = x.CountryName
                        }
                    )
                    .ToList()
                );
            }
            else
            {
                SeedJson<Country>(); // Also seeds Counties 
            }

            // Update Config Value
            //Seed(new List<Config>
            //{
            //    new Config()
            //    {
            //        Key = "site.display.counties",
            //        Value = !seedUN ? "true" : "false"
            //    }
            //});
        }

        private void SeedMaterialTypes()
        {
            var validGroups = _db.MaterialTypeGroups.ToList();

            Seed(
                ReadJson<MaterialType>()
                    .Select(x => new MaterialType()
                    {
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                        MaterialTypeGroups = x.MaterialTypeGroups
                            ?.Select(y => validGroups.First(z => z.Value == y.Value))
                            .ToList()
                    })
                    .ToList()
            );
        }

        private void Seed<T>(ICollection<T> entities) where T : class
        {
            var set = _db.Set<T>();

            if (set.Any())
            {
                _logger.LogInformation($"{ typeof(T).Name }: { set.Count() } entries already exist");
            }
            else
            {
                var table = set.EntityType.GetTableName();
                var props = set.EntityType.GetProperties();

                // Commit Changes
                set.AddRange(entities);
                
                // Save Changes
                // Check If Table Has Identity Column, Hence Needing Identity Insert
                if (props.Any(x => x.ValueGenerated == ValueGenerated.OnAdd))
                {
                    using var transaction = _db.Database.BeginTransaction();

                    _db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{table} ON");
                    _db.SaveChanges();
                    _db.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{table} OFF");
                        
                    transaction.Commit();
                }
                else
                {
                    _db.SaveChanges();
                }

                _logger.LogInformation($"{ typeof(T).Name }: Written { entities.Count() } entries");
            }
        }

        private void SeedJson<T>() where T : class
        {
            Seed(ReadJson<T>());
        }

        private static ICollection<T> ReadJson<T>(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_dataDir, typeof(T).Name) + ".json";
            }

            using var stream = new StreamReader(filePath);
            using var reader = new JsonTextReader(stream);

            return new JsonSerializer().Deserialize<ICollection<T>>(reader);
        }
    }
}