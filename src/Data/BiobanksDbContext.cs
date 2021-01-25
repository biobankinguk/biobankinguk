using Biobanks.Entities.Api;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Biobanks.Data
{
    public class BiobanksDbContext : DbContext
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

        // API Data
        public DbSet<LiveDiagnosis> LiveDiagnoses { get; set; }
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
        
        public BiobanksDbContext(DbContextOptions options) : base(options) { }

        public class BiobanksDbContextFactory : IDesignTimeDbContextFactory<BiobanksDbContext>
        {
            /*  Required for DesignTime creation of the context. EF operations can be done from
             *  either Package Manager or via Dotnet EF Tools.
             *  
             *  For either option, the connection string of the database must be passed via
             *  CLI arguments.
             *  
             *  Package Manager - https://docs.microsoft.com/en-us/ef/core/cli/powershell
             *  >> <Migrations Command> -Args "<Connection-String>"
             *  
             *  dotnet CLI - https://docs.microsoft.com/en-us/ef/core/cli/dotnet
             *  >> dotnet ef <CLI Command> -- "<Connection-String>"
             */

            public BiobanksDbContext CreateDbContext(string[] args)
            {
                var options = new DbContextOptionsBuilder<BiobanksDbContext>();
                options.UseSqlServer(args[0], options => options.EnableRetryOnFailure());

                return new BiobanksDbContext(options.Options);
            }
        }
    }

}
