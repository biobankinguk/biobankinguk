using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Directory.Data;
using Biobanks.Entities.Data;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Directory.DataSeed.Services;
using System.Security.Cryptography.X509Certificates;
using Biobanks.Entities.Data.ReferenceData;
using Directory.DataSeed.Dto;
using Biobanks.Entities.Shared.ReferenceData;

namespace Directory.DataSeed.Services
{
    internal class SeedingService : IHostedService
    {
        private const string _dataDir = "data/";

        // Some tables depend on others existing first, so we have to leave them til last
        // And use custom methods to handle their relationships
        // It's initialised in the constructor, as it depends on member methods
        public Dictionary<string, Action> LeaveTilLast;

        public Dictionary<string, Type> ValidTables = new Dictionary<string, Type>
        {
            ["AccessConditions"] = typeof(AccessCondition),
            ["AgeRanges"] = typeof(AgeRange),
            ["AnnualStatisticGroups"] = typeof(AnnualStatisticGroup),
            ["AssociatedDataProcurementTimeframes"] = typeof(AssociatedDataProcurementTimeframe),
            ["AssociatedDataTypeGroups"] = typeof(AssociatedDataTypeGroup),
            ["CollectionPercentages"] = typeof(CollectionPercentage),
            ["CollectionPoints"] = typeof(CollectionPoint),
            ["CollectionStatus"] = typeof(CollectionStatus),
            ["CollectionTypes"] = typeof(CollectionType),
            ["ConsentRestrictions"] = typeof(ConsentRestriction),
            ["Counties"] = typeof(County),
            ["Countries"] = typeof(Country),
            ["SnomedTerm"] = typeof(SnomedTerm), // Should this be seeded here?
            ["DonorCounts"] = typeof(DonorCount),
            ["Funders"] = typeof(Funder),
            ["Sexes"] = typeof(Sex),
            ["HtaStatus"] = typeof(HtaStatus),
            ["MaterialTypes"] = typeof(MaterialType),
            ["MacroscopicAssessments"] = typeof(MacroscopicAssessment),
            ["StorageTemperatures"] = typeof(StorageTemperature),
            ["RegistrationReasons"] = typeof(RegistrationReason),
            ["SampleCollectionModes"] = typeof(SampleCollectionMode),
            ["ServiceOfferings"] = typeof(ServiceOffering),
            ["SopStatus"] = typeof(SopStatus)
        };

        private readonly ILogger<SeedingService> _logger;
        private readonly BiobanksDbContext _db;
        private readonly CountriesWebService _countriesWebService;

        public SeedingService(ILogger<SeedingService> logger, BiobanksDbContext db, CountriesWebService countriesWebService)
        {
            _logger = logger;
            _db = db;
            _countriesWebService = countriesWebService;

            // Populate LeaveTilLast in here, as it depends on member methods
            LeaveTilLast = new Dictionary<string, Action>
            { 
                
                ["AnnualStatistics"] = SeedAnnualStatistics,
                ["AssociatedDataTypes"] = SeedAssociatedDataTypes,
                ["Counties"] = SeedCounties
            };
        }

        private async void DataSeeding()
        {
            //todo handle existing data
            //todo look into only seeding certain tables. Only needs ot be done if this scenario comes up though
            foreach (var s in ValidTables)
             if (s.Key != "Countries") Seed(s.Key);
                
            //gives user option to get seed UN countries vs UK countries
            await SeedCountries();

            // Now do the ones we left
            foreach (var s in LeaveTilLast)
                s.Value();

            Console.WriteLine("All done!");
            Console.WriteLine("Please sanity check Counties and Countries.");
        }

        public void Seed(string tableName)
        {
            // Skip any we need to leave til the end,
            // and use custom methods for
            if (LeaveTilLast.Keys.Contains(tableName)) return;

            var entityType = ValidTables[tableName];

            //TODO check if there are any records - if so, log and move on
            //var existingEntities =  _db.Set(entityType).AsNoTracking();

            var entities = Read(tableName);

            Console.WriteLine($@"Writing {entities.Count()} entries to {tableName}");

            foreach (var x in entities)
                _db.Set(entityType).Add(x);

            _db.SaveChanges();
        }

        private void SeedAnnualStatistics()
        {
            Read("AnnualStatistics", typeof(AnnualStatistic))
                .Select(x => (AnnualStatistic)x)
                .ToList()
                .ForEach(x =>
                {
                    x.AnnualStatisticGroup = null; // Prevents foreign key duplications
                    _db.AnnualStatistics.Add(x);
                });

            _db.SaveChanges();
        }

        private void SeedAssociatedDataTypes()
        {
            Read("AssociatedDataTypes", typeof(AssociatedDataType))
                .Select(x => (AssociatedDataType)x)
                .ToList()
                .ForEach(x =>
                {
                    x.AssociatedDataTypeGroup = null; // Prevents foreign key duplications
                    _db.AssociatedDataTypes.Add(x);
                });

            _db.SaveChanges();
        }

        private void SeedCounties()
        {
            var configEntity = _db.Configs.FirstOrDefault(item => item.Key == "site.display.counties");
            if (configEntity.Value == "false") return;
            var tableName = "Counties";

            var csvRecords = Read(tableName);

            // ok, but we're gonna need to link Counties to Countries by Id
            // We also have to assume the ID's are right!
            // TODO: better system based on name lookup?

            var countries = _db.Countries.ToList();

            Console.WriteLine($@"Writing {csvRecords.Count()} entries to {tableName}");

            foreach (dynamic x in csvRecords)
                _db.Counties.Add(new County
                {
                    Name = x.Name,
                    Country = countries.Single(y => y.CountryId == x.CountryId)
                });

            _db.SaveChanges();
        }

        private async Task SeedCountries()
        {
            var configEntity = _db.Configs.FirstOrDefault(item => item.Key == "site.display.counties");
            var reply = Prompt.GetYesNo("Would you like to seed UN Countries?", false);
            if (reply) //Seed UN Countries
            {
                List<CountriesDTO> countries = await _countriesWebService.GetCountries();
                var tableName = "Countries";
                Console.WriteLine($@"Writing {countries.Count()} UN Country entries to {tableName}");

                foreach (CountriesDTO country in countries)
                    _db.Countries.Add(new Country
                    {
                        Name = country.CountryName
                    });
                configEntity.Value = "false";
                _db.SaveChanges();
               
            }
           else if (!reply) //Seed CSV Countries
           {
                Seed("Countries");
                configEntity.Value = "true";
                _db.SaveChanges();

            }

        }

        private List<object> Read(string tableName, Type type)
        {
            //TODO catch and write to console that file not found, but move on. Allow user to decide which tables to populate
            using (var reader = new StreamReader($"data/{tableName}.csv"))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords(type).ToList();
                }
            }
        }
        
        private List<object> Read(string tableName)
            => Read(tableName, ValidTables[tableName]);

        public async Task StartAsync(CancellationToken cancellationToken)
            => DataSeeding();

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

    }
}

