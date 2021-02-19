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
using Biobanks.DataSeed.Data;
using Biobanks.DataSeed.Transforms;
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
                SeedDirectoryConfig,
                SeedCountries,
                SeedJson<AccessCondition>,
                SeedJson<AgeRange>,
                SeedAnnualStatistics,
                SeedJson<AssociatedDataProcurementTimeframe>,
                SeedAssociatedDataTypes,
                SeedJson<CollectionPercentage>,
                SeedJson<CollectionPoint>,
                SeedJson<CollectionStatus>,
                SeedJson<CollectionType>,
                SeedJson<ConsentRestriction>,
                SeedJson<DonorCount>,
                SeedJson<Funder>,
                SeedJson<HtaStatus>,
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
                SeedJson<MaterialTypeGroup>,
                SeedMaterialTypes,
                SeedJson<Sex>,
                SeedJson<OntologyTerm>,
                SeedJson<StorageTemperature>,
                SeedJson<PreservationType>,

                /* Post Seeding Fixes */
                FixOrganisationUrls
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

        private void FixOrganisationUrls()
        {
            _logger.LogInformation("Attempting to fix organisation urls");

            _db.Organisations
                .ToList()
                .ForEach(org =>
                {
                    if (UrlTransformer.TryValidateUrl(org.Url, out var validUrl))
                    {
                        org.Url = validUrl;
                    }
                });
            
            _db.SaveChanges();
        }

        private void SeedAnnualStatistics()
        {
            var groups = ReadJson<AnnualStatisticGroup>();

            // Seed AnnualStatisticGroups
            Seed(groups.Select(x =>
                new AnnualStatisticGroup
                {
                    Id = x.Id,
                    Value = x.Value
                }
            ));

            // Seed AnnualStatistics
            Seed(groups.SelectMany(x =>
                x.AnnualStatistics.Select(y =>
                    new AnnualStatistic
                    {
                        Id = y.Id,
                        Value = y.Value,
                        AnnualStatisticGroupId= x.Id
                    }
                )
            ));
        }

        private void SeedAssociatedDataTypes()
        {
            var groups = ReadJson<AssociatedDataTypeGroup>();

            // Seed AssociatedDataTypeGroups
            Seed(groups.Select(x => 
                new AssociatedDataTypeGroup
                {
                    Id = x.Id,
                    Value = x.Value
                }
            ));

            // Seed AssociatedDataTypes
            Seed(groups.SelectMany(x =>
                x.AssociatedDataTypes.Select(y =>
                    new AssociatedDataType
                    {
                        Id = y.Id,
                        Value = y.Value,
                        AssociatedDataTypeGroupId = x.Id
                    }
                )
            ));
        }

        private void SeedCountries()
        {
            var seedUN = Prompt.GetYesNo("Would you like to seed UN Countries?", false);

            if (seedUN)
            {
                var countries = _countriesWebService.ListCountriesAsync().Result;

                Seed(countries.Select(x =>
                        new Country()
                        {
                            Value = x.CountryName
                        }
                    ),
                    identityInsert: false
                );
            }
            else
            {
                var countries = ReadJson<Country>();

                // Seed Countries
                Seed(countries.Select(x => 
                    new Country 
                    {
                        Id = x.Id,
                        Value =  x.Value
                    }
                ));

                // Seed Counties
                Seed(countries.SelectMany(x => 
                    x.Counties.Select(y => 
                        new County 
                        {
                            Id = y.Id,
                            Value = y.Value,
                            CountryId = x.Id
                        }
                    )
                ));
            }

            // Update Config Value
            var config = new Config() 
            {
                Key = "site.display.counties",
                Value = !seedUN ? "true" : "false"
            };

            if (_db.Configs.Any(x => x.Key == config.Key))
            {
                _db.Configs.Update(config);
            }
            else
            {
                _db.Configs.Add(config);
            }

            _db.SaveChanges();
        }

        private void SeedDirectoryConfig()
        {
            Seed(DirectoryConfigs.DefaultConfigs);
        }

        private void SeedMaterialTypes()
        {
            var validGroups = _db.MaterialTypeGroups.ToList();

            Seed(
                ReadJson<MaterialType>().Select(x =>
                    new MaterialType()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                        MaterialTypeGroups =
                            x.MaterialTypeGroups?
                                .Select(y => validGroups.First(z => z.Value == y.Value))
                                .ToList()
                    }
                )
            );
        }

        private void SeedOrganisationTypes()
        {
            Seed(new List<OrganisationType>
            {
                new ()
                {
                    OrganisationTypeId = 1,
                    Description = "Biobank",
                    SortOrder = 1
                }
            });
        }

        private void SeedJson<T>() where T : class
        {
            Seed(ReadJson<T>());
        }

        private void Seed<T>(IEnumerable<T> entities, bool identityInsert = true) where T : class
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
                if (identityInsert && props.Any(x => x.ValueGenerated == ValueGenerated.OnAdd))
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