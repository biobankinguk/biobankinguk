using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Directory.Data;
using Directory.Entity.Data;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Directory.DataSeed.Services;
using System.Security.Cryptography.X509Certificates;
using Directory.DataSeed.Dto;

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
            _logger = logger;
            _db = db;
            _countriesWebService = countriesWebService;

            //["AccessConditions"] = typeof(AccessCondition),
            //["AgeRanges"] = typeof(AgeRange),
            //["AnnualStatisticGroups"] = typeof(AnnualStatisticGroup),
            //["AssociatedDataProcurementTimeframes"] = typeof(AssociatedDataProcurementTimeframe),
            //["AssociatedDataTypeGroups"] = typeof(AssociatedDataTypeGroup),
            //["CollectionPercentages"] = typeof(CollectionPercentage),
            //["CollectionPoints"] = typeof(CollectionPoint),
            //["CollectionStatus"] = typeof(CollectionStatus),
            //["CollectionTypes"] = typeof(CollectionType),
            //["ConsentRestrictions"] = typeof(ConsentRestriction),
            //["Counties"] = typeof(County),
            //["Countries"] = typeof(Country),
            //["Diagnosis"] = typeof(Diagnosis),
            //["DonorCounts"] = typeof(DonorCount),
            //["Funders"] = typeof(Funder),
            //["Sexes"] = typeof(Sex),
            //["HtaStatus"] = typeof(HtaStatus),
            //["MaterialTypes"] = typeof(MaterialType),
            //["MacroscopicAssessments"] = typeof(MacroscopicAssessment),
            //["PreservationTypes"] = typeof(PreservationType),
            //["RegistrationReasons"] = typeof(RegistrationReason),
            //["SampleCollectionModes"] = typeof(SampleCollectionMode),
            //["ServiceOfferings"] = typeof(ServiceOffering),
            //["SopStatus"] = typeof(SopStatus)

            _seedActions = new List<Action>
            {
                SeedCsv<AccessCondition>,
                SeedCsv<AgeRange>,
                SeedAnnualStatistics,
                SeedAssociatedDataTypes,
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

        private void Seed<T>(IEnumerable<T> entities) where T : class
        {
            _db.Set<T>().AddRange(entities);
            _db.SaveChanges();
        }

        private void SeedCsv<T>() where T : class
        {
            Seed(ReadCsv<T>());
        }

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

        private async Task SeedCountries()
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

        //private async Task SeedCountries()
        //{
        //    var configEntity = _db.Configs.FirstOrDefault(item => item.Key == "site.display.counties");
        //    var reply = Prompt.GetYesNo("Would you like to seed UN Countries?", false);
        //    if (reply) //Seed UN Countries
        //    {
        //        List<CountriesDTO> countries = await _countriesWebService.GetCountries();
        //        var tableName = "Countries";
        //        Console.WriteLine($@"Writing {countries.Count()} UN Country entries to {tableName}");

        //        foreach (CountriesDTO country in countries)
        //            _db.Countries.Add(new Country
        //            {
        //                Name = country.CountryName
        //            });
        //        configEntity.Value = "false";
        //        _db.SaveChanges();

        //    }
        //   else if (!reply) //Seed CSV Countries
        //   {
        //        Seed("Countries");
        //        configEntity.Value = "true";
        //        _db.SaveChanges();
        //    }
        //}

        //private void SeedCounties()
        //{
        //    var configEntity = _db.Configs.FirstOrDefault(item => item.Key == "site.display.counties");
        //    if (configEntity.Value == "false") return;
        //    var tableName = "Counties";

        //    var csvRecords = Read(tableName);

        //    // ok, but we're gonna need to link Counties to Countries by Id
        //    // We also have to assume the ID's are right!
        //    // TODO: better system based on name lookup?

        //    var countries = _db.Countries.ToList();

        //    Console.WriteLine($@"Writing {csvRecords.Count()} entries to {tableName}");

        //    foreach (dynamic x in csvRecords)
        //        _db.Counties.Add(new County
        //        {
        //            Name = x.Name,
        //            Country = countries.Single(y => y.CountryId == x.CountryId)
        //        });

        //    _db.SaveChanges();
        //}

        //private async Task SeedCountries()
        //{
        //    var configEntity = _db.Configs.FirstOrDefault(item => item.Key == "site.display.counties");
        //    var reply = Prompt.GetYesNo("Would you like to seed UN Countries?", false);
        //    if (reply) //Seed UN Countries
        //    {
        //        List<CountriesDTO> countries = await _countriesWebService.GetCountries();
        //        var tableName = "Countries";
        //        Console.WriteLine($@"Writing {countries.Count()} UN Country entries to {tableName}");

        //        foreach (CountriesDTO country in countries)
        //            _db.Countries.Add(new Country
        //            {
        //                Name = country.CountryName
        //            });
        //        configEntity.Value = "false";
        //        _db.SaveChanges();

        //    }
        //   else if (!reply) //Seed CSV Countries
        //   {
        //        Seed("Countries");
        //        configEntity.Value = "true";
        //        _db.SaveChanges();
        //    }
        //}

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