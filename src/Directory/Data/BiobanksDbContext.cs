using System.Data.Entity;
using Entities.Data;
using Entities.Shared.ReferenceData;

namespace Directory.Data
{
    public class BiobanksDbContext : DbContext
    {
        // Token Logging
        public DbSet<TokenIssueRecord> TokenIssueRecords { get; set; }
        public DbSet<TokenValidationRecord> TokenValidationRecords { get; set; }

        //Organisation details
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationType> OrganisationTypes { get; set; }
        public DbSet<ServiceOffering> ServiceOfferings { get; set; }
        public DbSet<OrganisationServiceOffering> OrgServiceOfferings { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<OrganisationRegisterRequest> OrganisationRegisterRequests { get; set; }

        public DbSet<Funder> Funders { get; set; }

        //Logo Blob Storage
        public DbSet<Blob> Blobs { get; set; }

        //Capability details
        public DbSet<DiagnosisCapability> DiagnosisCapabilities { get; set; }
        public DbSet<SampleCollectionMode> SampleCollectionModes { get; set; }

        //Shared capability / collection details
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<AssociatedDataType> AssociatedDataTypes { get; set; }
        public DbSet<AssociatedDataProcurementTimeframe> AssociatedDataProcurementTimeframes { get; set; }

        //Collection details
        public DbSet<Collection> Collections { get; set; }
        public DbSet<AccessCondition> AccessConditions { get; set; }
        public DbSet<CollectionType> CollectionTypes { get; set; }
        public DbSet<CollectionStatus> CollectionStatuses { get; set; }
        public DbSet<CollectionPoint> CollectionPoints { get; set; }
        public DbSet<ConsentRestriction> ConsentRestrictions { get; set; }
        public DbSet<CollectionAssociatedData> CollectionAssociatedDatas { get; set; }
        public DbSet<CapabilityAssociatedData> CapabilityAssociatedDatas { get; set; }
        public DbSet<HtaStatus> HtaStatuses { get; set; }

        //Collection SampleSet details
        public DbSet<CollectionSampleSet> CollectionSampleSets { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<AgeRange> AgeRanges { get; set; }
        public DbSet<DonorCount> DonorCounts { get; set; }

        //Collection Material Preservation details
        public DbSet<MaterialDetail> MaterialDetails { get; set; }
        public virtual DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<PreservationType> PreservationTypes { get; set; }
        public DbSet<MacroscopicAssessment> MacroscopicAssessments { get; set; }
        public DbSet<CollectionPercentage> CollectionPercentages { get; set; }

        //Network details
        public DbSet<Network> Networks { get; set; }
        public DbSet<OrganisationNetwork> OrganisationNetworks { get; set; }
        public DbSet<SopStatus> SopStatuses { get; set; }
        public DbSet<NetworkRegisterRequest> NetworkRegisterRequests { get; set; }
        public DbSet<NetworkUser> NetworkUsers { get; set; }

        //Site Config
        public DbSet<Config> Configs { get; set; }

        // Address Information 
        public DbSet<County> Counties { get; set; }
        public DbSet<Country> Countries { get; set; }

        // Biobank annual stats
        public DbSet<AnnualStatistic> AnnualStatistics { get; set; }
        public DbSet<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
        public DbSet<OrganisationAnnualStatistic> OrganisationAnnualStatistics { get; set; }

        // Biobank reasons for registering
        public DbSet<RegistrationReason> RegistrationReasons { get; set; }
        public DbSet<OrganisationRegistrationReason> OrganisationRegistrationReasons { get; set; }

        // Biobank Publications
        public DbSet<Publication> Publications { get; set; }

        public BiobanksDbContext() : base("Biobanks") { }
        public BiobanksDbContext(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collection>().Property(f => f.StartDate).HasColumnType("datetime2");
            modelBuilder.Entity<Diagnosis>().HasIndex(x => x.Description).IsUnique();
        }
    }
}
