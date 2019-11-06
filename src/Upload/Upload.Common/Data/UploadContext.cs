using Common.Data.Upload;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public class UploadContext : DbContext
    {

        public UploadContext(DbContextOptions options) : base(options) { }

        //Data which might be moved
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Error> Errors { get; set; }

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
    }
}
