using System.Threading.Tasks;
using Common.Data.Identity;
using Common.Data.ReferenceData;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public class DirectoryContext : IdentityUserContext<DirectoryUser>, IConfigurationDbContext, IPersistedGrantDbContext
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

        public DbSet<AnnualStatisticGroup> AnnualStatisticGroups { get; set; } = null!;
        public DbSet<AccessCondition> AccessConditions { get; set; } = null!;
        public DbSet<AgeRange> AgeRanges { get; set; } = null!;
        public DbSet<AnnualStatistic> AnnualStatistics { get; set; } = null!;
        public DbSet<AssociatedDataProcurementTimeframe> AssociatedDataProcurementTimeframes { get; set; } = null!;
        public DbSet<AssociatedDataType> AssociatedDataTypes { get; set; } = null!;
        public DbSet<CollectionPercentage> CollectionPercentages { get; set; } = null!;
        public DbSet<CollectionPoint> CollectionPoints { get; set; } = null!;
        public DbSet<CollectionStatus> CollectionStatuses { get; set; } = null!;
        public DbSet<CollectionType> CollectionTypes { get; set; } = null!;
        public DbSet<ConsentRestriction> ConsentRestrictions { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<County> Counties { get; set; } = null!;
        public DbSet<DonorCount> DonorCounts { get; set; } = null!;
        public DbSet<Funder> Funders { get; set; } = null!;
        public DbSet<HtaStatus> HtaStatuses { get; set; } = null!;
        public DbSet<MacroscopicAssessment> MacroscopicAssessments { get; set; } = null!;
        public DbSet<MaterialType> MaterialTypes { get; set; } = null!;
        public DbSet<MaterialTypeGroup> MaterialTypeGroups { get; set; } = null!;
        public DbSet<MaterialTypeGroupMaterialType> MaterialTypeGroupMaterialTypes { get; set;} = null!;
        public DbSet<OntologyTerm> OntologyTerms { get; set; } = null!;
        public DbSet<ServiceOffering> ServiceOfferings { get; set; } = null!;
        public DbSet<Sex> Sexes { get; set; } = null!;
        public DbSet<SopStatus> SopStatuses { get; set; } = null!;
        public DbSet<StorageTemperature> StorageTemperatures { get; set; } = null!;
        public DbSet<TokenRecord> TokenRecords { get; set; } = null!;

        #region IdentityServer4

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<IdentityResource> IdentityResources { get; set; } = null!;
        public DbSet<ApiResource> ApiResources { get; set; } = null!;
        public DbSet<PersistedGrant> PersistedGrants { get; set; } = null!;
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; } = null!;

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

        private void ConfigureIdentityServer(ModelBuilder b)
        {
            b.ConfigurePersistedGrantContext(_opStoreOptions);
            b.ConfigureClientContext(_configStoreOptions);
            b.ConfigureResourcesContext(_configStoreOptions);
        }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureIdentityServer(builder);
            base.OnModelCreating(builder);

            builder.Entity<MaterialTypeGroupMaterialType>()
                .HasKey(m => new { m.MaterialTypeId, m.MaterialTypeGroupId });
            builder.Entity<MaterialTypeGroupMaterialType>()
                .HasOne(m => m.MaterialType)
                .WithMany(mt => mt.MaterialTypeGroupMaterialTypes)
                .HasForeignKey(m => m.MaterialTypeId);
            builder.Entity<MaterialTypeGroupMaterialType>()
                .HasOne(m => m.MaterialTypeGroup)
                .WithMany(mtg => mtg.MaterialTypeGroupMaterialTypes)
                .HasForeignKey(m => m.MaterialTypeGroupId);
        }

    }
}
