namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApiEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiveDiagnosis",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DateDiagnosed = c.DateTime(nullable: false),
                    DiagnosisCodeId = c.String(nullable: false, maxLength: 20),
                    DiagnosisCodeOntologyVersionId = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    IndividualReferenceId = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OntologyVersions", t => t.DiagnosisCodeOntologyVersionId, cascadeDelete: true)
                .Index(t => t.DiagnosisCodeOntologyVersionId);

            AddForeignKey("dbo.LiveDiagnosis", "DiagnosisCodeId", "dbo.SnomedTerms", "Id");
            CreateIndex("dbo.LiveDiagnosis", "DiagnosisCodeId");

            CreateTable(
                "dbo.OntologyVersions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                    OntologyId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ontologies", t => t.OntologyId, cascadeDelete: true)
                .Index(t => t.OntologyId);

            CreateTable(
                "dbo.Ontologies",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Errors",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Message = c.String(),
                    RecordIdentifiers = c.String(),
                    SubmissionId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Submissions", t => t.SubmissionId, cascadeDelete: true)
                .Index(t => t.SubmissionId);

            CreateTable(
                "dbo.Submissions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BiobankId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTime(nullable: false),
                    TotalRecords = c.Int(nullable: false),
                    RecordsProcessed = c.Int(nullable: false),
                    StatusId = c.Int(nullable: false),
                    StatusChangeTimestamp = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.StatusId);

            CreateTable(
                "dbo.Status",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Value = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SampleContentMethods",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.LiveSamples",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Barcode = c.String(nullable: false),
                    YearOfBirth = c.Int(),
                    AgeAtDonation = c.Int(),
                    MaterialTypeId = c.Int(nullable: false),
                    StorageTemperatureId = c.Int(),
                    DateCreated = c.DateTime(nullable: false, storeType: "date"),
                    ExtractionSiteId = c.String(maxLength: 20),
                    ExtractionSiteOntologyVersionId = c.Int(),
                    ExtractionProcedureId = c.String(maxLength: 20),
                    SampleContentId = c.String(maxLength: 20),
                    SampleContentMethodId = c.Int(),
                    SexId = c.Int(),
                    CollectionName = c.String(),
                    OrganisationId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    IndividualReferenceId = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OntologyVersions", t => t.ExtractionSiteOntologyVersionId)
                .ForeignKey("dbo.MaterialTypes", t => t.MaterialTypeId, cascadeDelete: true)
                .ForeignKey("dbo.SampleContentMethods", t => t.SampleContentMethodId)
                .ForeignKey("dbo.Sexes", t => t.SexId)
                .ForeignKey("dbo.StorageTemperatures", t => t.StorageTemperatureId)
                .Index(t => t.MaterialTypeId)
                .Index(t => t.StorageTemperatureId)
                .Index(t => t.ExtractionSiteOntologyVersionId)
                .Index(t => t.SampleContentMethodId)
                .Index(t => t.SexId);

            AddForeignKey("dbo.LiveSamples", "ExtractionProcedureId", "dbo.SnomedTerms", "Id");
            AddForeignKey("dbo.LiveSamples", "ExtractionSiteId", "dbo.SnomedTerms", "Id");
            AddForeignKey("dbo.LiveSamples", "SampleContentId", "dbo.SnomedTerms", "Id");
            CreateIndex("dbo.LiveSamples", "ExtractionProcedureId");
            CreateIndex("dbo.LiveSamples", "ExtractionSiteId");
            CreateIndex("dbo.LiveSamples", "SampleContentId");

            CreateTable(
                "dbo.StagedDiagnosis",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DateDiagnosed = c.DateTime(nullable: false),
                    DiagnosisCodeId = c.String(nullable: false, maxLength: 20),
                    DiagnosisCodeOntologyVersionId = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    IndividualReferenceId = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OntologyVersions", t => t.DiagnosisCodeOntologyVersionId, cascadeDelete: true)
                .Index(t => t.DiagnosisCodeOntologyVersionId);

            AddForeignKey("dbo.StagedDiagnosis", "DiagnosisCodeId", "dbo.SnomedTerms", "Id");
            CreateIndex("dbo.StagedDiagnosis", "DiagnosisCodeId");

            CreateTable(
                "dbo.StagedDiagnosisDeletes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.StagedSampleDeletes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.StagedSamples",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Barcode = c.String(nullable: false),
                    YearOfBirth = c.Int(),
                    AgeAtDonation = c.Int(),
                    MaterialTypeId = c.Int(nullable: false),
                    StorageTemperatureId = c.Int(),
                    DateCreated = c.DateTime(nullable: false, storeType: "date"),
                    ExtractionSiteId = c.String(maxLength: 20),
                    ExtractionSiteOntologyVersionId = c.Int(),
                    ExtractionProcedureId = c.String(maxLength: 20),
                    SampleContentId = c.String(maxLength: 20),
                    SampleContentMethodId = c.Int(),
                    SexId = c.Int(),
                    CollectionName = c.String(),
                    OrganisationId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    IndividualReferenceId = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OntologyVersions", t => t.ExtractionSiteOntologyVersionId)
                .ForeignKey("dbo.MaterialTypes", t => t.MaterialTypeId, cascadeDelete: true)
                .ForeignKey("dbo.SampleContentMethods", t => t.SampleContentMethodId)
                .ForeignKey("dbo.Sexes", t => t.SexId)
                .ForeignKey("dbo.StorageTemperatures", t => t.StorageTemperatureId)
                .Index(t => t.MaterialTypeId)
                .Index(t => t.StorageTemperatureId)
                .Index(t => t.ExtractionSiteOntologyVersionId)
                .Index(t => t.SampleContentMethodId)
                .Index(t => t.SexId);

            AddForeignKey("dbo.StagedSamples", "ExtractionProcedureId", "dbo.SnomedTerms", "Id");
            AddForeignKey("dbo.StagedSamples", "ExtractionSiteId", "dbo.SnomedTerms", "Id");
            AddForeignKey("dbo.StagedSamples", "SampleContentId", "dbo.SnomedTerms", "Id");
            CreateIndex("dbo.StagedSamples", "ExtractionSiteId");
            CreateIndex("dbo.StagedSamples", "ExtractionProcedureId");
            CreateIndex("dbo.StagedSamples", "SampleContentId");

            CreateTable(
                "dbo.StagedTreatmentDeletes",
                c => new
                {
                    Id = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.StagedTreatments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DateTreated = c.DateTime(nullable: false),
                    TreatmentCodeId = c.String(nullable: false, maxLength: 20),
                    TreatmentLocationId = c.Int(),
                    TreatmentCodeOntologyVersionId = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    IndividualReferenceId = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OntologyVersions", t => t.TreatmentCodeOntologyVersionId, cascadeDelete: true)
                .ForeignKey("dbo.TreatmentLocations", t => t.TreatmentLocationId)
                .Index(t => t.TreatmentLocationId)
                .Index(t => t.TreatmentCodeOntologyVersionId);

            AddForeignKey("dbo.StagedTreatments", "TreatmentCodeId", "dbo.SnomedTerms", "Id");
            CreateIndex("dbo.StagedTreatments", "TreatmentCodeId");

            CreateTable(
                "dbo.TreatmentLocations",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.LiveTreatments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DateTreated = c.DateTime(nullable: false),
                    TreatmentCodeId = c.String(nullable: false, maxLength: 20),
                    TreatmentLocationId = c.Int(),
                    TreatmentCodeOntologyVersionId = c.Int(nullable: false),
                    OrganisationId = c.Int(nullable: false),
                    SubmissionTimestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    IndividualReferenceId = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OntologyVersions", t => t.TreatmentCodeOntologyVersionId, cascadeDelete: true)
                .ForeignKey("dbo.TreatmentLocations", t => t.TreatmentLocationId)
                .Index(t => t.TreatmentLocationId)
                .Index(t => t.TreatmentCodeOntologyVersionId);

            AddForeignKey("dbo.LiveTreatments", "TreatmentCodeId", "dbo.SnomedTerms", "Id");
            CreateIndex("dbo.LiveTreatments", "TreatmentCodeId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.LiveTreatments", "TreatmentLocationId", "dbo.TreatmentLocations");
            DropForeignKey("dbo.LiveTreatments", "TreatmentCodeOntologyVersionId", "dbo.OntologyVersions");
            DropForeignKey("dbo.LiveTreatments", "TreatmentCodeId", "dbo.SnomedTerms");
            DropForeignKey("dbo.StagedTreatments", "TreatmentLocationId", "dbo.TreatmentLocations");
            DropForeignKey("dbo.StagedTreatments", "TreatmentCodeOntologyVersionId", "dbo.OntologyVersions");
            DropForeignKey("dbo.StagedTreatments", "TreatmentCodeId", "dbo.SnomedTerms");
            DropForeignKey("dbo.StagedSamples", "StorageTemperatureId", "dbo.StorageTemperatures");
            DropForeignKey("dbo.StagedSamples", "SexId", "dbo.Sexes");
            DropForeignKey("dbo.StagedSamples", "SampleContentMethodId", "dbo.SampleContentMethods");
            DropForeignKey("dbo.StagedSamples", "SampleContentId", "dbo.SnomedTerms");
            DropForeignKey("dbo.StagedSamples", "MaterialTypeId", "dbo.MaterialTypes");
            DropForeignKey("dbo.StagedSamples", "ExtractionSiteOntologyVersionId", "dbo.OntologyVersions");
            DropForeignKey("dbo.StagedSamples", "ExtractionSiteId", "dbo.SnomedTerms");
            DropForeignKey("dbo.StagedSamples", "ExtractionProcedureId", "dbo.SnomedTerms");
            DropForeignKey("dbo.StagedDiagnosis", "DiagnosisCodeOntologyVersionId", "dbo.OntologyVersions");
            DropForeignKey("dbo.StagedDiagnosis", "DiagnosisCodeId", "dbo.SnomedTerms");
            DropForeignKey("dbo.LiveSamples", "StorageTemperatureId", "dbo.StorageTemperatures");
            DropForeignKey("dbo.LiveSamples", "SexId", "dbo.Sexes");
            DropForeignKey("dbo.LiveSamples", "SampleContentMethodId", "dbo.SampleContentMethods");
            DropForeignKey("dbo.LiveSamples", "SampleContentId", "dbo.SnomedTerms");
            DropForeignKey("dbo.LiveSamples", "MaterialTypeId", "dbo.MaterialTypes");
            DropForeignKey("dbo.LiveSamples", "ExtractionSiteOntologyVersionId", "dbo.OntologyVersions");
            DropForeignKey("dbo.LiveSamples", "ExtractionSiteId", "dbo.SnomedTerms");
            DropForeignKey("dbo.LiveSamples", "ExtractionProcedureId", "dbo.SnomedTerms");
            DropForeignKey("dbo.Submissions", "StatusId", "dbo.Status");
            DropForeignKey("dbo.Errors", "SubmissionId", "dbo.Submissions");
            DropForeignKey("dbo.LiveDiagnosis", "DiagnosisCodeOntologyVersionId", "dbo.OntologyVersions");
            DropForeignKey("dbo.OntologyVersions", "OntologyId", "dbo.Ontologies");
            DropForeignKey("dbo.LiveDiagnosis", "DiagnosisCodeId", "dbo.SnomedTerms");
            DropIndex("dbo.LiveTreatments", new[] { "TreatmentCodeOntologyVersionId" });
            DropIndex("dbo.LiveTreatments", new[] { "TreatmentLocationId" });
            DropIndex("dbo.LiveTreatments", new[] { "TreatmentCodeId" });
            DropIndex("dbo.StagedTreatments", new[] { "TreatmentCodeOntologyVersionId" });
            DropIndex("dbo.StagedTreatments", new[] { "TreatmentLocationId" });
            DropIndex("dbo.StagedTreatments", new[] { "TreatmentCodeId" });
            DropIndex("dbo.StagedSamples", new[] { "SexId" });
            DropIndex("dbo.StagedSamples", new[] { "SampleContentMethodId" });
            DropIndex("dbo.StagedSamples", new[] { "SampleContentId" });
            DropIndex("dbo.StagedSamples", new[] { "ExtractionProcedureId" });
            DropIndex("dbo.StagedSamples", new[] { "ExtractionSiteOntologyVersionId" });
            DropIndex("dbo.StagedSamples", new[] { "ExtractionSiteId" });
            DropIndex("dbo.StagedSamples", new[] { "StorageTemperatureId" });
            DropIndex("dbo.StagedSamples", new[] { "MaterialTypeId" });
            DropIndex("dbo.StagedDiagnosis", new[] { "DiagnosisCodeOntologyVersionId" });
            DropIndex("dbo.StagedDiagnosis", new[] { "DiagnosisCodeId" });
            DropIndex("dbo.LiveSamples", new[] { "SexId" });
            DropIndex("dbo.LiveSamples", new[] { "SampleContentMethodId" });
            DropIndex("dbo.LiveSamples", new[] { "SampleContentId" });
            DropIndex("dbo.LiveSamples", new[] { "ExtractionProcedureId" });
            DropIndex("dbo.LiveSamples", new[] { "ExtractionSiteOntologyVersionId" });
            DropIndex("dbo.LiveSamples", new[] { "ExtractionSiteId" });
            DropIndex("dbo.LiveSamples", new[] { "StorageTemperatureId" });
            DropIndex("dbo.LiveSamples", new[] { "MaterialTypeId" });
            DropIndex("dbo.Submissions", new[] { "StatusId" });
            DropIndex("dbo.Errors", new[] { "SubmissionId" });
            DropIndex("dbo.OntologyVersions", new[] { "OntologyId" });
            DropIndex("dbo.LiveDiagnosis", new[] { "DiagnosisCodeOntologyVersionId" });
            DropIndex("dbo.LiveDiagnosis", new[] { "DiagnosisCodeId" });
            DropTable("dbo.LiveTreatments");
            DropTable("dbo.TreatmentLocations");
            DropTable("dbo.StagedTreatments");
            DropTable("dbo.StagedTreatmentDeletes");
            DropTable("dbo.StagedSamples");
            DropTable("dbo.StagedSampleDeletes");
            DropTable("dbo.StagedDiagnosisDeletes");
            DropTable("dbo.StagedDiagnosis");
            DropTable("dbo.LiveSamples");
            DropTable("dbo.SampleContentMethods");
            DropTable("dbo.Status");
            DropTable("dbo.Submissions");
            DropTable("dbo.Errors");
            DropTable("dbo.Ontologies");
            DropTable("dbo.OntologyVersions");
            DropTable("dbo.LiveDiagnosis");
        }
    }
}
