using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Directory.Data;
using Entities.Data;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Directory.DataSeed.Services;
using System.Security.Cryptography.X509Certificates;
using Directory.DataSeed.Dto;
using Entities.Shared.ReferenceData;

namespace Directory.DataSeed.Services
{
    internal class SeedingService : IHostedService
    {
        private const string _dataDir = "data/";

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
                SeedCountries,
                SeedCounties,
                SeedCsv<Diagnosis>,
                SeedCsv<DonorCount>,
                SeedCsv<Funder>,
                SeedCsv<Sex>,
                SeedCsv<HtaStatus>,
                SeedCsv<MaterialType>,
                SeedCsv<MacroscopicAssessment>,
                SeedCsv<PreservationType>,
                SeedCsv<RegistrationReason>,
                SeedCsv<SampleCollectionMode>,
                SeedCsv<ServiceOffering>,
                SeedCsv<SopStatus>
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
            _db.Set<T>().AddRange(entities);
            _db.SaveChanges();
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
                return reader.GetRecords<T>();
            }
        }
    }
}