using System.Data.Entity;
using Biobanks.Entities.Api;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.Analytics;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Directory.Data
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
        public DbSet<OrganisationServiceOffering> OrganisationServiceOfferings { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
        public DbSet<OrganisationRegisterRequest> OrganisationRegisterRequests { get; set; }

        public DbSet<Funder> Funders { get; set; }

        //Logo Blob Storage
        public DbSet<Blob> Blobs { get; set; }

        //Capability details
        public DbSet<DiagnosisCapability> DiagnosisCapabilities { get; set; }
        public DbSet<SampleCollectionMode> SampleCollectionModes { get; set; }

        //Shared capability / collection details
        public DbSet<AssociatedDataType> AssociatedDataTypes { get; set; }
        public DbSet<AssociatedDataProcurementTimeframe> AssociatedDataProcurementTimeframes { get; set; }

        //Collection details
        public DbSet<Collection> Collections { get; set; }
        public DbSet<AccessCondition> AccessConditions { get; set; }
        public DbSet<CollectionType> CollectionTypes { get; set; }
        public DbSet<CollectionStatus> CollectionStatuses { get; set; }
        public DbSet<ConsentRestriction> ConsentRestrictions { get; set; }
        public DbSet<CollectionAssociatedData> CollectionAssociatedDatas { get; set; }
        public DbSet<CapabilityAssociatedData> CapabilityAssociatedDatas { get; set; }

        //Collection SampleSet details
        public DbSet<SampleSet> SampleSets { get; set; }
        public DbSet<AgeRange> AgeRanges { get; set; }
        public DbSet<DonorCount> DonorCounts { get; set; }

        //Collection Material Preservation details
        public DbSet<MaterialDetail> MaterialDetails { get; set; }
        public DbSet<MacroscopicAssessment> MacroscopicAssessments { get; set; }
        public DbSet<CollectionPercentage> CollectionPercentages { get; set; }

        //Network details
        public DbSet<Network> Networks { get; set; }
        public DbSet<OrganisationNetwork> OrganisationNetworks { get; set; }
        public DbSet<SopStatus> SopStatuses { get; set; }
        public DbSet<NetworkRegisterRequest> NetworkRegisterRequests { get; set; }
        public DbSet<NetworkUser> NetworkUsers { get; set; }

        //Domain Registration Rules
        public DbSet<RegistrationDomainRule> RegistrationDomainRules { get; set; }

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
        public DbSet<Annotation> Annotations { get; set; }

        /* Shared Reference Data */
        public virtual DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<MaterialTypeGroup> MaterialTypeGroups { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<OntologyTerm> OntologyTerms { get; set; }
        public DbSet<SnomedTag> SnomedTags { get; set; }
        public DbSet<StorageTemperature> StorageTemperatures { get; set; }
        public DbSet<PreservationType> PreservationTypes { get; set; }
        

        /*  API Entities  */
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Error> Errors { get; set; }

        // Reference Data
        public DbSet<SampleContentMethod> SampleContentMethods { get; set; }
        public DbSet<TreatmentLocation> TreatmentLocations { get; set; }
        public DbSet<Ontology> Ontologies { get; set; }
        public DbSet<OntologyVersion> OntologyVersions { get; set; }

        // API Data
        public DbSet<LiveDiagnosis> Diagnoses { get; set; }
        public DbSet<StagedDiagnosis> StagedDiagnoses { get; set; }
        public DbSet<StagedDiagnosisDelete> StagedDiagnosisDeletes { get; set; }

        public DbSet<LiveTreatment> Treatments { get; set; }
        public DbSet<StagedTreatment> StagedTreatments { get; set; }
        public DbSet<StagedTreatmentDelete> StagedTreatmentDeletes { get; set; }

        public DbSet<LiveSample> Samples { get; set; }
        public DbSet<StagedSample> StagedSamples { get; set; }
        public DbSet<StagedSampleDelete> StagedSampleDeletes { get; set; }

        public DbSet<ApiClient> ApiClients { get; set; }

        public DbSet<ContentPage> ContentPages { get; set; }

        //Analytics
        public DbSet<DirectoryAnalyticEvent> DirectoryAnalyticEvents { get; set; }

        public DbSet<DirectoryAnalyticMetric> DirectoryAnalyticMetric { get; set; }

        public DbSet<OrganisationAnalytic> OrganisationAnalytics { get; set; }

        public BiobanksDbContext() : this("Biobanks") { }
        
        public BiobanksDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new BiobanksDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiClient>()
                .HasMany(c => c.Organisations)
                .WithMany(o => o.ApiClients)
                .Map(join =>
                {
                    join.MapLeftKey("ApiClientsId");
                    join.MapRightKey("OrganisationsOrganisationId");
                    join.ToTable("ApiClientOrganisation");
                });

            modelBuilder.Entity<Collection>()
                .Property(f => f.StartDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<MaterialTypeGroup>()
                .HasMany(t => t.MaterialTypes)
                .WithMany(g => g.MaterialTypeGroups)
                .Map(gt =>
                {
                    gt.MapLeftKey("MaterialTypeGroupsId");
                    gt.MapRightKey("MaterialTypesId");
                    gt.ToTable("MaterialTypeMaterialTypeGroup");
                });

            modelBuilder.Entity<Collection>()
                .HasMany(t => t.ConsentRestrictions)
                .WithMany(g => g.Collections)
                .Map(gt =>
                {
                    gt.MapLeftKey("CollectionsCollectionId");
                    gt.MapRightKey("ConsentRestrictionsId");
                    gt.ToTable("CollectionConsentRestriction");
                });

            modelBuilder.Entity<Funder>()
                .HasMany(t => t.Organisations)
                .WithMany(g => g.Funders)
                .Map(gt =>
                {
                    gt.MapLeftKey("FundersId");
                    gt.MapRightKey("OrganisationsOrganisationId");
                    gt.ToTable("FunderOrganisation");
                });

            modelBuilder.Entity<OntologyTerm>()
                .HasIndex(x => x.Value)
                .IsUnique();

            modelBuilder.Entity<Status>()
                .ToTable("Statuses");
        }
    }
}
