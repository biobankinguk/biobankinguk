using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Biobanks.Directory.Data;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using CsvHelper.Configuration;

namespace Biobanks.DataSeed.Services
{
    internal class SeedingService : IHostedService
    {
        private const string _dataDir = "data";

        private readonly ILogger<SeedingService> _logger;
        private readonly BiobanksDbContext _db;
        private readonly CountriesWebService _countriesWebService;
        private readonly CsvConfiguration _csvConfiguration;

        private IEnumerable<Action> _seedActions;

        public SeedingService(ILogger<SeedingService> logger, BiobanksDbContext db, CountriesWebService countriesWebService)
        {
            _db = db;
            _logger = logger;
            _countriesWebService = countriesWebService;

            // Configure CsvReader
            _csvConfiguration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                IgnoreReferences = true
            };

            // List Order Determines Seed Order
            _seedActions = new List<Action>
            {
                /* Directory Specific */
                SeedCountries,
                SeedCounties,
                SeedCsv<AccessCondition>,
                SeedCsv<AgeRange>,
                SeedCsv<AnnualStatisticGroup>,
                SeedAnnualStatistics,
                SeedCsv<AssociatedDataProcurementTimeframe>,
                SeedCsv<AssociatedDataTypeGroup>,
                SeedAssociatedDataTypes,
                SeedCsv<CollectionPercentage>,
                SeedCsv<CollectionPoint>,
                SeedCsv<CollectionStatus>,
                SeedCsv<CollectionType>,
                SeedCsv<ConsentRestriction>,
                SeedCsv<DonorCount>,
                SeedCsv<Funder>,
                SeedCsv<HtaStatus>,
                SeedCsv<MacroscopicAssessment>,
                SeedCsv<RegistrationReason>,
                SeedCsv<SampleCollectionMode>,
                SeedCsv<ServiceOffering>,
                SeedCsv<SopStatus>,
                
                /* API Specific */
                SeedCsv<Ontology>,
                SeedOntologyVersion,
                SeedCsv<SampleContentMethod>,
                SeedCsv<Status>,
                SeedCsv<TreatmentLocation>,

                /* Shared */
                SeedCsv<MaterialTypeGroup>,
                SeedCsv<MaterialType>,
                SeedCsv<Sex>,
                SeedCsv<OntologyTerm>,
                SeedCsv<StorageTemperature>,
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

        private void SeedAnnualStatistics() 
        {
            Seed(
                ReadCsv<AnnualStatistic>().Select(x =>
                    {
                        x.AnnualStatisticGroup = null;  // Prevents foreign key duplications
                        return x;
                    }
                )
            );
        }

        private void SeedAssociatedDataTypes() 
        {
            Seed(
                ReadCsv<AssociatedDataType>().Select(x =>
                    {
                        x.AssociatedDataTypeGroup = null;  // Prevents foreign key duplications
                        return x;
                    }
                )
            );
        }

        private void SeedCountries() 
        {
            var seedUN = Prompt.GetYesNo("Would you like to seed UN Countries?", false);
            
            // Seed Countries
            if (seedUN)
            {
                Seed(
                    _countriesWebService.ListCountriesAsync().Result.Select(x => new Country
                        {
                            Value = x.CountryName
                        }
                    )
                );
            }
            else
            {
                SeedCsv<Country>();
            }

            // Update Config Value
            WriteConfig("site.display.counties", !seedUN ? "true" : "false");
        }

        private void SeedCounties()
        {
            if (ReadConfig("site.display.counties") == "true")
            {
                // References Of All Seeded Countries
                var countries = _db.Set<Country>().ToList();

                // Seed Counties From Csv - Linking With Existing Countries
                Seed(
                    ReadCsv<County>().Select(x => new County
                    {
                        Value = x.Value,
                        Country = countries.Single(y => y.Id == x.CountryId)
                    })
                );
            }
        }

        private void SeedOntologyVersion()
        {
            var ontologies = _db.Set<Ontology>().ToList();

            Seed(
                ReadCsv<OntologyVersion>()
                    .Select(x =>
                    {
                        x.Ontology = ontologies.FirstOrDefault(y => y.Id == x.OntologyId);
                        return x;
                    })
                    .Where(x => x.Ontology != null)
            );
        }

        private void Seed<T>(IEnumerable<T> entities) where T : class
        {
            var set = _db.Set<T>();
            
            if (set.Any())
            {
                _logger.LogInformation($"{ typeof(T).Name }: { set.Count() } entries already exist");
            }
            else
            {
                _logger.LogInformation($"{ typeof(T).Name }: Writing { entities.Count() } entries");
                set.AddRange(entities);
                _db.SaveChanges();
            }
        }

        private void SeedCsv<T>() where T : class
        {
            Seed(ReadCsv<T>());
        }

        private void WriteConfig(string key, string value)
        {
            _db.Configs.FirstOrDefault(x => x.Key == key).Value = value;
            _db.SaveChanges();
        }

        private string ReadConfig(string key)
        {
            return _db.Configs.FirstOrDefault(x => x.Key == key).Value;
        }

        private IEnumerable<T> ReadCsv<T>(string filePath = "") where T : class
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_dataDir, typeof(T).Name) + ".csv";
            }

            using (var stream = new StreamReader(filePath))
            using (var reader = new CsvReader(stream, _csvConfiguration))
            {
                return reader.GetRecords<T>().ToList();
            }
        }
    }
}