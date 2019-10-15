using System.Threading.Tasks;
using Common.Data.ReferenceData;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public class DirectoryContext : DbContext, IConfigurationDbContext, IPersistedGrantDbContext
    {
        private readonly OperationalStoreOptions _opStoreOptions;
        private readonly ConfigurationStoreOptions _configStoreOptions;

        public DirectoryContext(
            DbContextOptions<DirectoryContext> options,
            OperationalStoreOptions opStoreOptions,
            ConfigurationStoreOptions configStoreOptions)
            : base(options)
        {
            _opStoreOptions = opStoreOptions;
            _configStoreOptions = configStoreOptions;
        }

        public DbSet<AccessCondition> AccessConditions { get; set; }
        public DbSet<AgeRange> AgeRanges { get; set; }
        public DbSet<AnnualStatistic> AnnualStatistics { get; set; }
        public DbSet<AssociatedDataProcurementTimeframe> AssociatedDataProcurementTimeframes { get; set; }
        public DbSet<AssociatedDataType> AssociatedDataTypes { get; set; }
        public DbSet<CollectionPercentage> CollectionPercentages { get; set; }
        public DbSet<CollectionPoint> CollectionPoints { get; set; }
        public DbSet<CollectionStatus> CollectionStatuses { get; set; }
        public DbSet<CollectionType> CollectionTypes { get; set; }
        public DbSet<ConsentRestriction> ConsentRestrictions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<DonorCount> DonorCounts { get; set; }
        public DbSet<Funder> Funders { get; set; }
        public DbSet<HtaStatus> HtaStatuses { get; set; }
        public DbSet<MacroscopicAssessment> MacroscopicAssessments { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<OntologyTerm> OntologyTerms { get; set; }
        public DbSet<ServiceOffering> ServiceOfferings { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<SopStatus> SopStatuses { get; set; }
        public DbSet<StorageTemperature> StorageTemperatures { get; set; }

        #region IdentityServer4

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<IdentityResource> IdentityResources { get; set; } = null!;
        public DbSet<ApiResource> ApiResources { get; set; } = null!;
        public DbSet<PersistedGrant> PersistedGrants { get; set; } = null!;
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; } = null!;

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigurePersistedGrantContext(_opStoreOptions);
            builder.ConfigureClientContext(_configStoreOptions);
            builder.ConfigureResourcesContext(_configStoreOptions);

            base.OnModelCreating(builder);

            builder.Entity<DeviceFlowCodes>().HasNoKey();
            builder.Entity<PersistedGrant>().HasNoKey();
        }
    }
}
