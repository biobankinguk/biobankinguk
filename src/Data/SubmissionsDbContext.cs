using Biobanks.Common.Data.Entities;
using Biobanks.Common.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Data
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

        }
        public SubmissionsDbContext(DbContextOptions options) : base(options) { }

        public class SubmissionsDbContextFactory : IDesignTimeDbContextFactory<SubmissionsDbContext>
        {
            /*  Required for DesignTime creation of the context.
             *
             *  For running migration ensure at least EFCore 5.0.0 is used
             *  >> Install-Package Microsoft.EntityFrameworkCore - Version 5.0.0
             *  >> Install-Package Microsoft.EntityFrameworkCore.Tools -Version 5.0.0
             *  
             *  The connection string is passed via CLI arguments
             *  >> Update-Database -Args "<Connection-String>"
             */

            public SubmissionsDbContext CreateDbContext(string[] args)
            {
                var options = new DbContextOptionsBuilder<SubmissionsDbContext>();
                options.UseSqlServer(args[0], options => options.EnableRetryOnFailure());

                return new SubmissionsDbContext(options.Options);
            }
        }
    }

}
