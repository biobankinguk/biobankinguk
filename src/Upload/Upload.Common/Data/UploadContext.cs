using Microsoft.EntityFrameworkCore;
using Upload.Common.Data.Entities;

namespace Upload.Common.Data
{
    public class UploadContext : DbContext
    {

        public UploadContext(DbContextOptions options) : base(options) { }

        //Data which might be moved
        public DbSet<Submission> Submissions { get; set; } = null!;
        public DbSet<Error> Errors { get; set; } = null!;

        // Actual Data
        public DbSet<LiveDiagnosis> Diagnoses { get; set; } = null!;
        public DbSet<StagedDiagnosis> StagedDiagnoses { get; set; } = null!;
        public DbSet<StagedDiagnosisDelete> StagedDiagnosisDeletes { get; set; } = null!;

        public DbSet<LiveTreatment> Treatments { get; set; } = null!;
        public DbSet<StagedTreatment> StagedTreatments { get; set; } = null!;
        public DbSet<StagedTreatmentDelete> StagedTreatmentDeletes { get; set; } = null!;

        public DbSet<LiveSample> Samples { get; set; } = null!;
        public DbSet<StagedSample> StagedSamples { get; set; } = null!;
        public DbSet<StagedSampleDelete> StagedSampleDeletes { get; set; } = null!;

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
    }
}
