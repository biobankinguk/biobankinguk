﻿using Biobanks.Common.Data.Entities;
using Biobanks.Common.Data.Entities.ReferenceData;
using LegacyData.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;

namespace LegacyData
{
    public class SubmissionsDbContext : DbContext
    {
        public DbSet<Status> Statuses { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Error> Errors { get; set; }

        // Reference Data
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<MaterialTypeGroup> MaterialTypeGroups { get; set; }
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

            //Join entity keys
            model.Entity<MaterialTypeMaterialTypeGroup>()
                .HasKey(x => new
                {
                    x.MaterialTypeId,
                    x.MaterialTypeGroupId
                });

        }

        public SubmissionsDbContext(DbContextOptions options) : base(options) { }

    }

}