using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobanks.Directory.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Newtonsoft.Json;

namespace Biobanks.DataSeed.Services
{
    internal class SeedingService : IHostedService
    {
        private const string _dataDir = "data";

        private readonly ILogger<SeedingService> _logger;
        private readonly BiobanksDbContext _db;
        private readonly CountriesWebService _countriesWebService;

        private IEnumerable<Action> _seedActions;

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
                SeedJson<MaterialType>,
                SeedJson<Sex>,
                SeedJson<OntologyTerm>,
                SeedJson<StorageTemperature>,
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
                    _countriesWebService.ListCountriesAsync().Result.Select(x => new Country
                        {
                            Value = x.CountryName
                        }
                    )
                );
            }
            else
            {
                SeedJson<Country>(); // Also seeds Counties 
            }

            // Update Config Value
            _db.Configs.FirstOrDefault(x => x.Key == "site.display.counties").Value = (!seedUN ? "true" : "false");
            _db.SaveChanges();
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

        private void SeedJson<T>() where T : class
        {
            Seed(ReadJson<T>());
        }

        private IEnumerable<T> ReadJson<T>(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_dataDir, typeof(T).Name) + ".json";
            }

            using (var stream = new StreamReader(filePath))
            using (var reader = new JsonTextReader(stream))
            {
                return new JsonSerializer().Deserialize<IEnumerable<T>>(reader);
            }
        }
    }
}