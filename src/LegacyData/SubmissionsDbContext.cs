using Biobanks.Entities.Api;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Microsoft.EntityFrameworkCore;

// Legacy Entities Used To Support EF 3.1
using LiveSample = Biobanks.LegacyData.Entities.LiveSample;
using StagedSample = Biobanks.LegacyData.Entities.StagedSample;
using MaterialType = Biobanks.LegacyData.Entities.MaterialType;
using MaterialTypeGroup = Biobanks.LegacyData.Entities.MaterialTypeGroup;
using MaterialTypeMaterialTypeGroup = Biobanks.LegacyData.Entities.JoinEntities.MaterialTypeMaterialTypeGroup;

namespace Biobanks.LegacyData
{
    public class BiobanksDbContext : DbContext
    {
        public DbSet<Status> Statuses { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Error> Errors { get; set; }

        // Reference Data
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<MaterialTypeGroup> MaterialTypeGroups { get; set; }
        public DbSet<MaterialTypeMaterialTypeGroup> MaterialTypeMaterialTypeGroup { get; set; }
        public DbSet<SampleContentMethod> SampleContentMethods { get; set; }
        public DbSet<SnomedTerm> SnomedTerms { get; set; }
        public DbSet<SnomedTag> SnomedTags { get; set; }
        public DbSet<StorageTemperature> StorageTemperatures { get; set; }
        public DbSet<TreatmentLocation> TreatmentLocations { get; set; }
        public DbSet<Ontology> Ontologies { get; set; }
        public DbSet<OntologyVersion> OntologyVersions { get; set; }
        public DbSet<Sex> Sexes { get; set; }

        // Actual Data
        public DbSet<LiveDiagnosis> Diagnoses { get; set; }
        public DbSet<StagedDiagnosis> StagedDiagnoses { get; set; }
        public DbSet<StagedDiagnosisDelete> StagedDiagnosisDeletes { get; set; }

        public DbSet<LiveTreatment> Treatments { get; set; }
        public DbSet<StagedTreatment> StagedTreatments { get; set; }
        public DbSet<StagedTreatmentDelete> StagedTreatmentDeletes { get; set; }

        public DbSet<LiveSample> Samples { get; set; }
        public DbSet<StagedSample> StagedSamples { get; set; }
        public DbSet<StagedSampleDelete> StagedSampleDeletes { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            // Indices (for unique constraints)
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

            model.Entity<Entities.LiveSample>()
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

            model.Entity<Entities.StagedSample>()
                .HasIndex(x => new
                {
                    x.OrganisationId,
                    x.IndividualReferenceId,
                    x.Barcode,
                    x.CollectionName
                }).IsUnique();

            // Join Entity Many-Many Relationship - Required for EF 3.1
            model.Entity<Entities.JoinEntities.MaterialTypeMaterialTypeGroup>()
                .HasKey(x => new
                {
                    x.MaterialTypeId,
                    x.MaterialTypeGroupId
                });

            model.Entity<Entities.JoinEntities.MaterialTypeMaterialTypeGroup>()
                .HasOne(x => x.MaterialType)
                .WithMany(x => x.MaterialTypeGroups)
                .HasForeignKey(x => x.MaterialTypeId);

            model.Entity<Entities.JoinEntities.MaterialTypeMaterialTypeGroup>()
                .HasOne(x => x.MaterialTypeGroup)
                .WithMany(x => x.MaterialTypes)
                .HasForeignKey(x => x.MaterialTypeGroupId);
        }

        public BiobanksDbContext(DbContextOptions options) : base(options) { }

    }

}