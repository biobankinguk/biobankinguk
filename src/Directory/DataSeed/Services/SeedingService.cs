using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Directory.Data;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Entities.Data;
using Entities.Shared.ReferenceData;
using Entities.Api.ReferenceData;

namespace Directory.DataSeed.Services
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

            _seedActions = new List<Action>
            {
                /* Directory Specific */
                SeedCsv<AccessCondition>,
                SeedCsv<AgeRange>,
                SeedAnnualStatistics,
                SeedCsv<AnnualStatisticGroup>,
                SeedCsv<AssociatedDataProcurementTimeframe>,
                SeedAssociatedDataTypes,
                SeedCsv<AssociatedDataTypeGroup>,
                SeedCsv<CollectionPercentage>,
                SeedCsv<CollectionPoint>,
                SeedCsv<CollectionStatus>,
                SeedCsv<CollectionType>,
                SeedCsv<ConsentRestriction>,
                //SeedCountries,
                //SeedCounties,
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
                //SeedCsv<OntologyVersion>,
                SeedCsv<SampleContentMethod>,
                SeedCsv<Status>,
                SeedCsv<TreatmentLocation>,

                /* Shared */
                SeedCsv<MaterialType>,
                SeedCsv<Sex>,
                //SeedCsv<SnomedTerm>,
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
                    _countriesWebService.ListCountries().Select(x => new Country
                        {
                            Name = x.CountryName
                        }
                    )
                );
            }
            else
            {
                SeedCsv<Country>();
            }

            // Update Config Value
        }

        private void SeedCounties()
        {
            var countries = _db.Set<Country>().ToList();

            // Seed Counties From Csv - Linking With Existing Countries
            Seed(
                ReadCsv<County>().Select(x => new County
                {
                    Name = x.Name,
                    Country = countries.Single(y => y.CountryId == x.CountryId)
                })
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

        private IEnumerable<T> ReadCsv<T>(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = $"{ _dataDir }/{ typeof(T).Name }.csv";
            }

            using (var stream = new StreamReader(filePath))
            using (var reader = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                return reader.GetRecords<T>().ToList();
            }
        }
    }
}