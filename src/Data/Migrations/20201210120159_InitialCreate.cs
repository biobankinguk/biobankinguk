using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ontologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ontologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleContentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleContentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SnomedTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnomedTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedDiagnosisDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedDiagnosisDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedSampleDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedSampleDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedTreatmentDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedTreatmentDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageTemperatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageTemperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypeMaterialTypeGroup",
                columns: table => new
                {
                    MaterialTypeGroupsId = table.Column<int>(type: "int", nullable: false),
                    MaterialTypesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeMaterialTypeGroup", x => new { x.MaterialTypeGroupsId, x.MaterialTypesId });
                    table.ForeignKey(
                        name: "FK_MaterialTypeMaterialTypeGroup_MaterialTypeGroups_MaterialTypeGroupsId",
                        column: x => x.MaterialTypeGroupsId,
                        principalTable: "MaterialTypeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialTypeMaterialTypeGroup_MaterialTypes_MaterialTypesId",
                        column: x => x.MaterialTypesId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OntologyVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OntologyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OntologyVersions_Ontologies_OntologyId",
                        column: x => x.OntologyId,
                        principalTable: "Ontologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SnomedTerms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnomedTagId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnomedTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnomedTerms_SnomedTags_SnomedTagId",
                        column: x => x.SnomedTagId,
                        principalTable: "SnomedTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BiobankId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    RecordsProcessed = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    StatusChangeTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateDiagnosed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiagnosisCodeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagnoses_OntologyVersions_DiagnosisCodeOntologyVersionId",
                        column: x => x.DiagnosisCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Diagnoses_SnomedTerms_DiagnosisCodeId",
                        column: x => x.DiagnosisCodeId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YearOfBirth = table.Column<int>(type: "int", nullable: true),
                    AgeAtDonation = table.Column<int>(type: "int", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(type: "int", nullable: true),
                    ExtractionProcedureId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SampleContentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SampleContentMethodId = table.Column<int>(type: "int", nullable: true),
                    SexId = table.Column<int>(type: "int", nullable: true),
                    CollectionName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Samples_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Samples_OntologyVersions_ExtractionSiteOntologyVersionId",
                        column: x => x.ExtractionSiteOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_SampleContentMethods_SampleContentMethodId",
                        column: x => x.SampleContentMethodId,
                        principalTable: "SampleContentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_SnomedTerms_ExtractionProcedureId",
                        column: x => x.ExtractionProcedureId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_SnomedTerms_ExtractionSiteId",
                        column: x => x.ExtractionSiteId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_SnomedTerms_SampleContentId",
                        column: x => x.SampleContentId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StagedDiagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateDiagnosed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiagnosisCodeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedDiagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedDiagnoses_OntologyVersions_DiagnosisCodeOntologyVersionId",
                        column: x => x.DiagnosisCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedDiagnoses_SnomedTerms_DiagnosisCodeId",
                        column: x => x.DiagnosisCodeId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StagedSamples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YearOfBirth = table.Column<int>(type: "int", nullable: true),
                    AgeAtDonation = table.Column<int>(type: "int", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(type: "int", nullable: true),
                    ExtractionProcedureId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SampleContentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SampleContentMethodId = table.Column<int>(type: "int", nullable: true),
                    SexId = table.Column<int>(type: "int", nullable: true),
                    CollectionName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedSamples_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyVersions_ExtractionSiteOntologyVersionId",
                        column: x => x.ExtractionSiteOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_SampleContentMethods_SampleContentMethodId",
                        column: x => x.SampleContentMethodId,
                        principalTable: "SampleContentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_SnomedTerms_ExtractionProcedureId",
                        column: x => x.ExtractionProcedureId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_SnomedTerms_ExtractionSiteId",
                        column: x => x.ExtractionSiteId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_SnomedTerms_SampleContentId",
                        column: x => x.SampleContentId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StagedTreatments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateTreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TreatmentCodeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TreatmentLocationId = table.Column<int>(type: "int", nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedTreatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedTreatments_OntologyVersions_TreatmentCodeOntologyVersionId",
                        column: x => x.TreatmentCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedTreatments_SnomedTerms_TreatmentCodeId",
                        column: x => x.TreatmentCodeId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedTreatments_TreatmentLocations_TreatmentLocationId",
                        column: x => x.TreatmentLocationId,
                        principalTable: "TreatmentLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Treatments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateTreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TreatmentCodeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TreatmentLocationId = table.Column<int>(type: "int", nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Treatments_OntologyVersions_TreatmentCodeOntologyVersionId",
                        column: x => x.TreatmentCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatments_SnomedTerms_TreatmentCodeId",
                        column: x => x.TreatmentCodeId,
                        principalTable: "SnomedTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatments_TreatmentLocations_TreatmentLocationId",
                        column: x => x.TreatmentLocationId,
                        principalTable: "TreatmentLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordIdentifiers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Diagnoses_DiagnosisCodeOntologyVersionId",
                table: "Diagnoses",
                column: "DiagnosisCodeOntologyVersionId");

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
                name: "IX_MaterialTypeMaterialTypeGroup_MaterialTypesId",
                table: "MaterialTypeMaterialTypeGroup",
                column: "MaterialTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyVersions_OntologyId",
                table: "OntologyVersions",
                column: "OntologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_ExtractionProcedureId",
                table: "Samples",
                column: "ExtractionProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_ExtractionSiteId",
                table: "Samples",
                column: "ExtractionSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_ExtractionSiteOntologyVersionId",
                table: "Samples",
                column: "ExtractionSiteOntologyVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_MaterialTypeId",
                table: "Samples",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "Samples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true,
                filter: "[CollectionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_SampleContentId",
                table: "Samples",
                column: "SampleContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_SampleContentMethodId",
                table: "Samples",
                column: "SampleContentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_SexId",
                table: "Samples",
                column: "SexId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_StorageTemperatureId",
                table: "Samples",
                column: "StorageTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SnomedTerms_SnomedTagId",
                table: "SnomedTerms",
                column: "SnomedTagId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_DiagnosisCodeId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_DiagnosisCodeOntologyVersionId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeOntologyVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_OrganisationId_IndividualReferenceId_DateDiagnosed_DiagnosisCodeId",
                table: "StagedDiagnoses",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateDiagnosed", "DiagnosisCodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_ExtractionProcedureId",
                table: "StagedSamples",
                column: "ExtractionProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_ExtractionSiteId",
                table: "StagedSamples",
                column: "ExtractionSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_ExtractionSiteOntologyVersionId",
                table: "StagedSamples",
                column: "ExtractionSiteOntologyVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_MaterialTypeId",
                table: "StagedSamples",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "StagedSamples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true,
                filter: "[CollectionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_SampleContentId",
                table: "StagedSamples",
                column: "SampleContentId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_SampleContentMethodId",
                table: "StagedSamples",
                column: "SampleContentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_SexId",
                table: "StagedSamples",
                column: "SexId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_StorageTemperatureId",
                table: "StagedSamples",
                column: "StorageTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedTreatments_OrganisationId_IndividualReferenceId_DateTreated_TreatmentCodeId",
                table: "StagedTreatments",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateTreated", "TreatmentCodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StagedTreatments_TreatmentCodeId",
                table: "StagedTreatments",
                column: "TreatmentCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedTreatments_TreatmentCodeOntologyVersionId",
                table: "StagedTreatments",
                column: "TreatmentCodeOntologyVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedTreatments_TreatmentLocationId",
                table: "StagedTreatments",
                column: "TreatmentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StatusId",
                table: "Submissions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_OrganisationId_IndividualReferenceId_DateTreated_TreatmentCodeId",
                table: "Treatments",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "DateTreated", "TreatmentCodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_TreatmentCodeId",
                table: "Treatments",
                column: "TreatmentCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_TreatmentCodeOntologyVersionId",
                table: "Treatments",
                column: "TreatmentCodeOntologyVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatments_TreatmentLocationId",
                table: "Treatments",
                column: "TreatmentLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "MaterialTypeMaterialTypeGroup");

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
                name: "MaterialTypeGroups");

            migrationBuilder.DropTable(
                name: "MaterialTypes");

            migrationBuilder.DropTable(
                name: "SampleContentMethods");

            migrationBuilder.DropTable(
                name: "Sexes");

            migrationBuilder.DropTable(
                name: "StorageTemperatures");

            migrationBuilder.DropTable(
                name: "OntologyVersions");

            migrationBuilder.DropTable(
                name: "SnomedTerms");

            migrationBuilder.DropTable(
                name: "TreatmentLocations");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Ontologies");

            migrationBuilder.DropTable(
                name: "SnomedTags");
        }
    }
}
