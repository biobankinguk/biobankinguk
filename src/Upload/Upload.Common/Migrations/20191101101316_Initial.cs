using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Upload.Common.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OmopTerm",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TagId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OmopTerm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    IndividualReferenceId = table.Column<string>(maxLength: 255, nullable: false),
                    Barcode = table.Column<string>(nullable: false),
                    YearOfBirth = table.Column<int>(nullable: true),
                    AgeAtDonation = table.Column<int>(nullable: true),
                    MaterialTypeId = table.Column<int>(nullable: false),
                    StorageTemperatureId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(nullable: true),
                    ExtractionProcedureId = table.Column<string>(nullable: true),
                    SampleContentId = table.Column<string>(nullable: true),
                    SampleContentMethodId = table.Column<int>(nullable: true),
                    SexId = table.Column<int>(nullable: true),
                    CollectionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedDiagnosisDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedDiagnosisDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedSampleDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedSampleDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedSamples",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    IndividualReferenceId = table.Column<string>(maxLength: 255, nullable: false),
                    Barcode = table.Column<string>(nullable: false),
                    YearOfBirth = table.Column<int>(nullable: true),
                    AgeAtDonation = table.Column<int>(nullable: true),
                    MaterialTypeId = table.Column<int>(nullable: false),
                    StorageTemperatureId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(nullable: true),
                    ExtractionProcedureId = table.Column<string>(nullable: true),
                    SampleContentId = table.Column<string>(nullable: true),
                    SampleContentMethodId = table.Column<int>(nullable: true),
                    SexId = table.Column<int>(nullable: true),
                    CollectionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedSamples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedTreatmentDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedTreatmentDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedTreatments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    IndividualReferenceId = table.Column<string>(maxLength: 255, nullable: false),
                    DateTreated = table.Column<DateTime>(nullable: false),
                    TreatmentCodeId = table.Column<string>(nullable: false),
                    TreatmentLocationId = table.Column<int>(nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedTreatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Treatments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    IndividualReferenceId = table.Column<string>(maxLength: 255, nullable: false),
                    DateTreated = table.Column<DateTime>(nullable: false),
                    TreatmentCodeId = table.Column<string>(nullable: false),
                    TreatmentLocationId = table.Column<int>(nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    IndividualReferenceId = table.Column<string>(maxLength: 255, nullable: false),
                    DateDiagnosed = table.Column<DateTime>(nullable: false),
                    DiagnosisCodeId = table.Column<string>(nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagnoses_OmopTerm_DiagnosisCodeId",
                        column: x => x.DiagnosisCodeId,
                        principalTable: "OmopTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StagedDiagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    IndividualReferenceId = table.Column<string>(maxLength: 255, nullable: false),
                    DateDiagnosed = table.Column<DateTime>(nullable: false),
                    DiagnosisCodeId = table.Column<string>(nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedDiagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedDiagnoses_OmopTerm_DiagnosisCodeId",
                        column: x => x.DiagnosisCodeId,
                        principalTable: "OmopTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BiobankId = table.Column<int>(nullable: false),
                    SubmissionTimestamp = table.Column<DateTime>(nullable: false),
                    TotalRecords = table.Column<int>(nullable: false),
                    RecordsProcessed = table.Column<int>(nullable: false),
                    UploadStatusId = table.Column<int>(nullable: true),
                    StatusChangeTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_UploadStatuses_UploadStatusId",
                        column: x => x.UploadStatusId,
                        principalTable: "UploadStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(nullable: true),
                    RecordIdentifiers = table.Column<string>(nullable: true),
                    SubmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Errors_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_DiagnosisCodeId",
                table: "Diagnoses",
                column: "DiagnosisCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_OrganisationId_IndividualReferenceId_DateDiagnosed_DiagnosisCodeId",
                table: "Diagnoses",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateDiagnosed", "DiagnosisCodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Errors_SubmissionId",
                table: "Errors",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "Samples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true,
                filter: "[CollectionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_DiagnosisCodeId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_OrganisationId_IndividualReferenceId_DateDiagnosed_DiagnosisCodeId",
                table: "StagedDiagnoses",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateDiagnosed", "DiagnosisCodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "StagedSamples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true,
                filter: "[CollectionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StagedTreatments_OrganisationId_IndividualReferenceId_DateTreated_TreatmentCodeId",
                table: "StagedTreatments",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateTreated", "TreatmentCodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UploadStatusId",
                table: "Submissions",
                column: "UploadStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_OrganisationId_IndividualReferenceId_DateTreated_TreatmentCodeId",
                table: "Treatments",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateTreated", "TreatmentCodeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "Samples");

            migrationBuilder.DropTable(
                name: "StagedDiagnoses");

            migrationBuilder.DropTable(
                name: "StagedDiagnosisDeletes");

            migrationBuilder.DropTable(
                name: "StagedSampleDeletes");

            migrationBuilder.DropTable(
                name: "StagedSamples");

            migrationBuilder.DropTable(
                name: "StagedTreatmentDeletes");

            migrationBuilder.DropTable(
                name: "StagedTreatments");

            migrationBuilder.DropTable(
                name: "Treatments");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "OmopTerm");

            migrationBuilder.DropTable(
                name: "UploadStatuses");
        }
    }
}
