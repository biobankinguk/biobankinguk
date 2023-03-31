using Biobanks.Data.Entities;
using Biobanks.Data.Entities.Analytics;
using Biobanks.Data.Entities.Api;
using Biobanks.Data.Entities.Api.ReferenceData;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Biobanks.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    #region Reference Data: API
    public DbSet<Ontology> Ontologies { get; set; }
    public DbSet<OntologyVersion> OntologyVersions { get; set; }
    public DbSet<SampleContentMethod> SampleContentMethods { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<TreatmentLocation> TreatmentLocations { get; set; }
    #endregion

    #region Reference Data: Directory
    public DbSet<AccessCondition> AccessConditions { get; set; }
    public DbSet<AgeRange> AgeRanges { get; set; }
    public DbSet<AnnualStatistic> AnnualStatistics { get; set; }
    public DbSet<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
    public DbSet<AssociatedDataProcurementTimeframe> AssociatedDataProcurementTimeframes { get; set; }
    public DbSet<AssociatedDataType> AssociatedDataTypes { get; set; }
    public DbSet<AssociatedDataTypeGroup> AssociatedDataTypeGroups { get; set; }
    public DbSet<CollectionPercentage> CollectionPercentages { get; set; }
    public DbSet<CollectionStatus> CollectionStatus { get; set; }
    public DbSet<CollectionType> CollectionTypes { get; set; }
    public DbSet<ConsentRestriction> ConsentRestrictions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<County> Counties { get; set; }
    public DbSet<DonorCount> DonorCounts { get; set; }
    public DbSet<Funder> Funders { get; set; }
    public DbSet<MacroscopicAssessment> MacroscopicAssessments { get; set; }
    public DbSet<RegistrationReason> RegistrationReasons { get; set; }
    public DbSet<SampleCollectionMode> SampleCollectionModes { get; set; }
    public DbSet<ServiceOffering> ServiceOfferings { get; set; }
    public DbSet<SopStatus> SopStatus { get; set; }
    #endregion

    #region Reference Data: Shared
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<MaterialTypeGroup> MaterialTypeGroups { get; set; }
    public DbSet<OntologyTerm> OntologyTerms { get; set; }
    public DbSet<PreservationType> PreservationTypes { get; set; }
    public DbSet<Sex> Sexes { get; set; }
    public DbSet<SnomedTag> SnomedTags { get; set; }
    public DbSet<StorageTemperature> StorageTemperatures { get; set; }
    #endregion

    #region Application Data: Submissions Service
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Error> Errors { get; set; }

    public DbSet<LiveDiagnosis> Diagnoses { get; set; }
    public DbSet<StagedDiagnosis> StagedDiagnoses { get; set; }
    public DbSet<StagedDiagnosisDelete> StagedDiagnosisDeletes { get; set; }

    public DbSet<LiveTreatment> Treatments { get; set; }
    public DbSet<StagedTreatment> StagedTreatments { get; set; }
    public DbSet<StagedTreatmentDelete> StagedTreatmentDeletes { get; set; }

    public DbSet<LiveSample> Samples { get; set; }
    public DbSet<StagedSample> StagedSamples { get; set; }
    public DbSet<StagedSampleDelete> StagedSampleDeletes { get; set; }
    #endregion

    #region Application Data: Directory
    public DbSet<Blob> Blobs { get; set; }
    public DbSet<CapabilityAssociatedData> CapabilityAssociatedDatas { get; set; }
    public DbSet<CollectionAssociatedData> CollectionAssociatedDatas { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<SampleSet> SampleSets { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<RegistrationDomainRule> RegistrationDomainRules { get; set; }
    public DbSet<DiagnosisCapability> DiagnosisCapabilities { get; set; }
    public DbSet<MaterialDetail> MaterialDetails { get; set; }
    public DbSet<Network> Networks { get; set; }
    public DbSet<NetworkRegisterRequest> NetworkRegisterRequests { get; set; }
    public DbSet<NetworkUser> NetworkUsers { get; set; }
    public DbSet<Organisation> Organisations { get; set; }
    public DbSet<OrganisationAnnualStatistic> OrganisationAnnualStatistics { get; set; }
    public DbSet<OrganisationNetwork> OrganisationNetworks { get; set; }
    public DbSet<OrganisationRegistrationReason> OrganisationRegistrationReasons { get; set; }
    public DbSet<OrganisationRegisterRequest> OrganisationRegisterRequests { get; set; }
    public DbSet<OrganisationType> OrganisationTypes { get; set; }
    public DbSet<OrganisationUser> OrganisationUsers { get; set; }
    public DbSet<OrganisationServiceOffering> OrganisationServiceOfferings { get; set; }
    public DbSet<TokenIssueRecord> TokenIssueRecords { get; set; }
    public DbSet<TokenValidationRecord> TokenValidationRecords { get; set; }
    public DbSet<ContentPage> ContentPages { get; set; }
    #endregion

    #region Application Data: Publications
    public DbSet<Annotation> Annotations { get; set; }
    public DbSet<Publication> Publications { get; set; }
    #endregion

    #region Analytics

    public DbSet<DirectoryAnalyticEvent> DirectoryAnalyticEvents { get; set; }
    public DbSet<DirectoryAnalyticMetric> DirectoryAnalyticMetrics { get; set; }
    public DbSet<OrganisationAnalytic> OrganisationAnalytics { get; set; }

    #endregion

    public DbSet<ApiClient> ApiClients { get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
      base.OnModelCreating(model);

      // Join Tables
      model.Entity<MaterialTypeGroup>()
          .HasMany(x => x.MaterialTypes)
          .WithMany(y => y.MaterialTypeGroups);

      // Composite Primary Keys
      model.Entity<CapabilityAssociatedData>()
          .HasKey(x => new
          {
            x.DiagnosisCapabilityId,
            x.AssociatedDataTypeId
          });

      model.Entity<CollectionAssociatedData>()
          .HasKey(x => new
          {
            x.CollectionId,
            x.AssociatedDataTypeId
          });

      model.Entity<NetworkUser>()
          .HasKey(x => new
          {
            x.NetworkId,
            x.NetworkUserId
          });

      model.Entity<OrganisationAnnualStatistic>()
          .HasKey(x => new
          {
            x.OrganisationId,
            x.AnnualStatisticId,
            x.Year
          });

      model.Entity<OrganisationNetwork>()
          .HasKey(x => new
          {
            x.OrganisationId,
            x.NetworkId
          });

      model.Entity<OrganisationRegistrationReason>()
          .HasKey(x => new
          {
            x.OrganisationId,
            x.RegistrationReasonId
          });

      model.Entity<OrganisationServiceOffering>()
          .HasKey(x => new
          {
            x.OrganisationId,
            x.ServiceOfferingId
          });

      model.Entity<OrganisationUser>()
          .HasKey(x => new
          {
            x.OrganisationId,
            x.OrganisationUserId
          });

      // Indices (for unique constraints)
      model.Entity<MaterialDetail>()
          .HasIndex(x => new
          {
            x.SampleSetId,
            x.MaterialTypeId,
            x.StorageTemperatureId,
            x.MacroscopicAssessmentId,
            x.ExtractionProcedureId,
            x.PreservationTypeId
          }).IsUnique();

      model.Entity<LiveDiagnosis>()
          .HasIndex(x => new
          {
            x.OrganisationId,
            x.IndividualReferenceId,
            x.DateDiagnosed,
            x.DiagnosisCodeId
          }).IsUnique();

      model.Entity<LiveTreatment>()
          .HasIndex(x => new
          {
            x.OrganisationId,
            x.IndividualReferenceId,
            x.DateTreated,
            x.TreatmentCodeId
          }).IsUnique();

      model.Entity<LiveSample>()
          .HasIndex(x => new
          {
            x.OrganisationId,
            x.IndividualReferenceId,
            x.Barcode,
            x.CollectionName
          }).IsUnique();

      model.Entity<StagedDiagnosis>()
          .HasIndex(x => new
          {
            x.OrganisationId,
            x.IndividualReferenceId,
            x.DateDiagnosed,
            x.DiagnosisCodeId
          }).IsUnique();

      model.Entity<StagedTreatment>()
          .HasIndex(x => new
          {
            x.OrganisationId,
            x.IndividualReferenceId,
            x.DateTreated,
            x.TreatmentCodeId
          }).IsUnique();

      model.Entity<StagedSample>()
          .HasIndex(x => new
          {
            x.OrganisationId,
            x.IndividualReferenceId,
            x.Barcode,
            x.CollectionName
          }).IsUnique();

      model.Entity<AgeRange>()
          .HasIndex(x => new
          {
            x.LowerBound,
            x.UpperBound
          }).IsUnique();


    }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }
  }
}
