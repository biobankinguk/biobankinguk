using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class Inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgeRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Annotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualStatisticGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualStatisticGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataProcurementTimeframes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayValue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataProcurementTimeframes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentDisposition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionPercentages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LowerBound = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpperBound = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionPercentages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "ConsentRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonorCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LowerBound = table.Column<int>(type: "int", nullable: true),
                    UpperBound = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonorCounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HtaStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtaStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MacroscopicAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroscopicAssessments", x => x.Id);
                });

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
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetworkRegisterRequests",
                columns: table => new
                {
                    NetworkRegisterRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetworkName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NetworkCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkRegisterRequests", x => x.NetworkRegisterRequestId);
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
                name: "OrganisationTypes",
                columns: table => new
                {
                    OrganisationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationTypes", x => x.OrganisationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleCollectionModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleCollectionModes", x => x.Id);
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
                name: "ServiceOfferings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOfferings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
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
                name: "SopStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SopStatus", x => x.Id);
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
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageTemperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenIssueRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenIssueRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenValidationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidationSuccessful = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenValidationRecords", x => x.Id);
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
                name: "AnnualStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnnualStatisticGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualStatistics_AnnualStatisticGroups_AnnualStatisticGroupId",
                        column: x => x.AnnualStatisticGroupId,
                        principalTable: "AnnualStatisticGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssociatedDataTypeGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociatedDataTypes_AssociatedDataTypeGroups_AssociatedDataTypeGroupId",
                        column: x => x.AssociatedDataTypeGroupId,
                        principalTable: "AssociatedDataTypeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Counties_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "OrganisationRegisterRequests",
                columns: table => new
                {
                    OrganisationRegisterRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisationTypeId = table.Column<int>(type: "int", nullable: false),
                    OrganisationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganisationCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganisationExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRegisterRequests", x => x.OrganisationRegisterRequestId);
                    table.ForeignKey(
                        name: "FK_OrganisationRegisterRequests_OrganisationTypes_OrganisationTypeId",
                        column: x => x.OrganisationTypeId,
                        principalTable: "OrganisationTypes",
                        principalColumn: "OrganisationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTerms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnomedTagId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OntologyTerms_SnomedTags_SnomedTagId",
                        column: x => x.SnomedTagId,
                        principalTable: "SnomedTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    NetworkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SopStatusId = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactHandoverEnabled = table.Column<bool>(type: "bit", nullable: false),
                    HandoverBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandoverOrgIdsUrlParamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MultipleHandoverOrdIdsParams = table.Column<bool>(type: "bit", nullable: false),
                    HandoverNonMembers = table.Column<bool>(type: "bit", nullable: false),
                    HandoverNonMembersUrlParamName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.NetworkId);
                    table.ForeignKey(
                        name: "FK_Networks_SopStatus_SopStatusId",
                        column: x => x.SopStatusId,
                        principalTable: "SopStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "PreservationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    OrganisationExternalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountyId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    OrganisationTypeId = table.Column<int>(type: "int", nullable: false),
                    GoverningInstitution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoverningDepartment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SharingOptOut = table.Column<bool>(type: "bit", nullable: false),
                    EthicsRegistration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HtaLicence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnonymousIdentifier = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OtherRegistrationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcludePublications = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.OrganisationId);
                    table.ForeignKey(
                        name: "FK_Organisations_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisations_OrganisationTypes_OrganisationTypeId",
                        column: x => x.OrganisationTypeId,
                        principalTable: "OrganisationTypes",
                        principalColumn: "OrganisationTypeId",
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
                    DiagnosisCodeId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagnoses_OntologyTerms_DiagnosisCodeId",
                        column: x => x.DiagnosisCodeId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Diagnoses_OntologyVersions_DiagnosisCodeOntologyVersionId",
                        column: x => x.DiagnosisCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    DiagnosisCodeId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedDiagnoses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedDiagnoses_OntologyTerms_DiagnosisCodeId",
                        column: x => x.DiagnosisCodeId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedDiagnoses_OntologyVersions_DiagnosisCodeOntologyVersionId",
                        column: x => x.DiagnosisCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    TreatmentCodeId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    TreatmentLocationId = table.Column<int>(type: "int", nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedTreatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedTreatments_OntologyTerms_TreatmentCodeId",
                        column: x => x.TreatmentCodeId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedTreatments_OntologyVersions_TreatmentCodeOntologyVersionId",
                        column: x => x.TreatmentCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
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
                    TreatmentCodeId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    TreatmentLocationId = table.Column<int>(type: "int", nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Treatments_OntologyTerms_TreatmentCodeId",
                        column: x => x.TreatmentCodeId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatments_OntologyVersions_TreatmentCodeOntologyVersionId",
                        column: x => x.TreatmentCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
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
                name: "NetworkUsers",
                columns: table => new
                {
                    NetworkId = table.Column<int>(type: "int", nullable: false),
                    NetworkUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkUsers", x => new { x.NetworkId, x.NetworkUserId });
                    table.ForeignKey(
                        name: "FK_NetworkUsers_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Networks",
                        principalColumn: "NetworkId",
                        onDelete: ReferentialAction.Cascade);
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
                    PreservationTypeId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(type: "int", nullable: true),
                    ExtractionProcedureId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    SampleContentId = table.Column<string>(type: "nvarchar(20)", nullable: true),
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
                        name: "FK_Samples_OntologyTerms_ExtractionProcedureId",
                        column: x => x.ExtractionProcedureId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_OntologyTerms_ExtractionSiteId",
                        column: x => x.ExtractionSiteId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_OntologyTerms_SampleContentId",
                        column: x => x.SampleContentId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_OntologyVersions_ExtractionSiteOntologyVersionId",
                        column: x => x.ExtractionSiteOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_PreservationTypes_PreservationTypeId",
                        column: x => x.PreservationTypeId,
                        principalTable: "PreservationTypes",
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
                        name: "FK_Samples_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    PreservationTypeId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(type: "int", nullable: true),
                    ExtractionProcedureId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    SampleContentId = table.Column<string>(type: "nvarchar(20)", nullable: true),
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
                        name: "FK_StagedSamples_OntologyTerms_ExtractionProcedureId",
                        column: x => x.ExtractionProcedureId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyTerms_ExtractionSiteId",
                        column: x => x.ExtractionSiteId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyTerms_SampleContentId",
                        column: x => x.SampleContentId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyVersions_ExtractionSiteOntologyVersionId",
                        column: x => x.ExtractionSiteOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StagedSamples_PreservationTypes_PreservationTypeId",
                        column: x => x.PreservationTypeId,
                        principalTable: "PreservationTypes",
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
                        name: "FK_StagedSamples_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    OntologyTermId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromApi = table.Column<bool>(type: "bit", nullable: false),
                    HtaStatusId = table.Column<int>(type: "int", nullable: true),
                    AccessConditionId = table.Column<int>(type: "int", nullable: false),
                    CollectionTypeId = table.Column<int>(type: "int", nullable: true),
                    CollectionStatusId = table.Column<int>(type: "int", nullable: false),
                    CollectionPointId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionId);
                    table.ForeignKey(
                        name: "FK_Collections_AccessConditions_AccessConditionId",
                        column: x => x.AccessConditionId,
                        principalTable: "AccessConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collections_CollectionPoints_CollectionPointId",
                        column: x => x.CollectionPointId,
                        principalTable: "CollectionPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collections_CollectionStatus_CollectionStatusId",
                        column: x => x.CollectionStatusId,
                        principalTable: "CollectionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collections_CollectionTypes_CollectionTypeId",
                        column: x => x.CollectionTypeId,
                        principalTable: "CollectionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Collections_HtaStatus_HtaStatusId",
                        column: x => x.HtaStatusId,
                        principalTable: "HtaStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Collections_OntologyTerms_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Collections_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosisCapabilities",
                columns: table => new
                {
                    DiagnosisCapabilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    OntologyTermId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SampleCollectionModeId = table.Column<int>(type: "int", nullable: false),
                    AnnualDonorExpectation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisCapabilities", x => x.DiagnosisCapabilityId);
                    table.ForeignKey(
                        name: "FK_DiagnosisCapabilities_OntologyTerms_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiagnosisCapabilities_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagnosisCapabilities_SampleCollectionModes_SampleCollectionModeId",
                        column: x => x.SampleCollectionModeId,
                        principalTable: "SampleCollectionModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunderOrganisation",
                columns: table => new
                {
                    FundersId = table.Column<int>(type: "int", nullable: false),
                    OrganisationsOrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunderOrganisation", x => new { x.FundersId, x.OrganisationsOrganisationId });
                    table.ForeignKey(
                        name: "FK_FunderOrganisation_Funders_FundersId",
                        column: x => x.FundersId,
                        principalTable: "Funders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunderOrganisation_Organisations_OrganisationsOrganisationId",
                        column: x => x.OrganisationsOrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationAnnualStatistics",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    AnnualStatisticId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAnnualStatistics", x => new { x.OrganisationId, x.AnnualStatisticId, x.Year });
                    table.ForeignKey(
                        name: "FK_OrganisationAnnualStatistics_AnnualStatistics_AnnualStatisticId",
                        column: x => x.AnnualStatisticId,
                        principalTable: "AnnualStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationAnnualStatistics_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationNetworks",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    NetworkId = table.Column<int>(type: "int", nullable: false),
                    ExternalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationNetworks", x => new { x.OrganisationId, x.NetworkId });
                    table.ForeignKey(
                        name: "FK_OrganisationNetworks_Networks_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Networks",
                        principalColumn: "NetworkId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationNetworks_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRegistrationReasons",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    RegistrationReasonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRegistrationReasons", x => new { x.OrganisationId, x.RegistrationReasonId });
                    table.ForeignKey(
                        name: "FK_OrganisationRegistrationReasons_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationRegistrationReasons_RegistrationReasons_RegistrationReasonId",
                        column: x => x.RegistrationReasonId,
                        principalTable: "RegistrationReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationServiceOfferings",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    ServiceOfferingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationServiceOfferings", x => new { x.OrganisationId, x.ServiceOfferingId });
                    table.ForeignKey(
                        name: "FK_OrganisationServiceOfferings_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationServiceOfferings_ServiceOfferings_ServiceOfferingId",
                        column: x => x.ServiceOfferingId,
                        principalTable: "ServiceOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUsers",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    OrganisationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUsers", x => new { x.OrganisationId, x.OrganisationUserId });
                    table.ForeignKey(
                        name: "FK_OrganisationUsers_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Publications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Authors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Journal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    DOI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accepted = table.Column<bool>(type: "bit", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnnotationsSynced = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publications_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionAssociatedDatas",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    AssociatedDataTypeId = table.Column<int>(type: "int", nullable: false),
                    AssociatedDataProcurementTimeframeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionAssociatedDatas", x => new { x.CollectionId, x.AssociatedDataTypeId });
                    table.ForeignKey(
                        name: "FK_CollectionAssociatedDatas_AssociatedDataProcurementTimeframes_AssociatedDataProcurementTimeframeId",
                        column: x => x.AssociatedDataProcurementTimeframeId,
                        principalTable: "AssociatedDataProcurementTimeframes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectionAssociatedDatas_AssociatedDataTypes_AssociatedDataTypeId",
                        column: x => x.AssociatedDataTypeId,
                        principalTable: "AssociatedDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionAssociatedDatas_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionConsentRestriction",
                columns: table => new
                {
                    CollectionsCollectionId = table.Column<int>(type: "int", nullable: false),
                    ConsentRestrictionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionConsentRestriction", x => new { x.CollectionsCollectionId, x.ConsentRestrictionsId });
                    table.ForeignKey(
                        name: "FK_CollectionConsentRestriction_Collections_CollectionsCollectionId",
                        column: x => x.CollectionsCollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionConsentRestriction_ConsentRestrictions_ConsentRestrictionsId",
                        column: x => x.ConsentRestrictionsId,
                        principalTable: "ConsentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionSampleSets",
                columns: table => new
                {
                    SampleSetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    SexId = table.Column<int>(type: "int", nullable: false),
                    AgeRangeId = table.Column<int>(type: "int", nullable: false),
                    DonorCountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionSampleSets", x => x.SampleSetId);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_AgeRanges_AgeRangeId",
                        column: x => x.AgeRangeId,
                        principalTable: "AgeRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_DonorCounts_DonorCountId",
                        column: x => x.DonorCountId,
                        principalTable: "DonorCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CapabilityAssociatedDatas",
                columns: table => new
                {
                    DiagnosisCapabilityId = table.Column<int>(type: "int", nullable: false),
                    AssociatedDataTypeId = table.Column<int>(type: "int", nullable: false),
                    AssociatedDataProcurementTimeframeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapabilityAssociatedDatas", x => new { x.DiagnosisCapabilityId, x.AssociatedDataTypeId });
                    table.ForeignKey(
                        name: "FK_CapabilityAssociatedDatas_AssociatedDataProcurementTimeframes_AssociatedDataProcurementTimeframeId",
                        column: x => x.AssociatedDataProcurementTimeframeId,
                        principalTable: "AssociatedDataProcurementTimeframes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapabilityAssociatedDatas_AssociatedDataTypes_AssociatedDataTypeId",
                        column: x => x.AssociatedDataTypeId,
                        principalTable: "AssociatedDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CapabilityAssociatedDatas_DiagnosisCapabilities_DiagnosisCapabilityId",
                        column: x => x.DiagnosisCapabilityId,
                        principalTable: "DiagnosisCapabilities",
                        principalColumn: "DiagnosisCapabilityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnotationPublication",
                columns: table => new
                {
                    AnnotationsId = table.Column<int>(type: "int", nullable: false),
                    PublicationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotationPublication", x => new { x.AnnotationsId, x.PublicationsId });
                    table.ForeignKey(
                        name: "FK_AnnotationPublication_Annotations_AnnotationsId",
                        column: x => x.AnnotationsId,
                        principalTable: "Annotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnotationPublication_Publications_PublicationsId",
                        column: x => x.PublicationsId,
                        principalTable: "Publications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialDetails",
                columns: table => new
                {
                    SampleSetId = table.Column<int>(type: "int", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "int", nullable: false),
                    MacroscopicAssessmentId = table.Column<int>(type: "int", nullable: false),
                    PreservationTypeId = table.Column<int>(type: "int", nullable: true),
                    CollectionPercentageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDetails", x => new { x.SampleSetId, x.MaterialTypeId, x.StorageTemperatureId, x.MacroscopicAssessmentId });
                    table.ForeignKey(
                        name: "FK_MaterialDetails_CollectionPercentages_CollectionPercentageId",
                        column: x => x.CollectionPercentageId,
                        principalTable: "CollectionPercentages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_CollectionSampleSets_SampleSetId",
                        column: x => x.SampleSetId,
                        principalTable: "CollectionSampleSets",
                        principalColumn: "SampleSetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_MacroscopicAssessments_MacroscopicAssessmentId",
                        column: x => x.MacroscopicAssessmentId,
                        principalTable: "MacroscopicAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                        column: x => x.PreservationTypeId,
                        principalTable: "PreservationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationPublication_PublicationsId",
                table: "AnnotationPublication",
                column: "PublicationsId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualStatistics_AnnualStatisticGroupId",
                table: "AnnualStatistics",
                column: "AnnualStatisticGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociatedDataTypes_AssociatedDataTypeGroupId",
                table: "AssociatedDataTypes",
                column: "AssociatedDataTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityAssociatedDatas_AssociatedDataProcurementTimeframeId",
                table: "CapabilityAssociatedDatas",
                column: "AssociatedDataProcurementTimeframeId");

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityAssociatedDatas_AssociatedDataTypeId",
                table: "CapabilityAssociatedDatas",
                column: "AssociatedDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionAssociatedDatas_AssociatedDataProcurementTimeframeId",
                table: "CollectionAssociatedDatas",
                column: "AssociatedDataProcurementTimeframeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionAssociatedDatas_AssociatedDataTypeId",
                table: "CollectionAssociatedDatas",
                column: "AssociatedDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionConsentRestriction_ConsentRestrictionsId",
                table: "CollectionConsentRestriction",
                column: "ConsentRestrictionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_AccessConditionId",
                table: "Collections",
                column: "AccessConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CollectionPointId",
                table: "Collections",
                column: "CollectionPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CollectionStatusId",
                table: "Collections",
                column: "CollectionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CollectionTypeId",
                table: "Collections",
                column: "CollectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_HtaStatusId",
                table: "Collections",
                column: "HtaStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_OntologyTermId",
                table: "Collections",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_OrganisationId",
                table: "Collections",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_AgeRangeId",
                table: "CollectionSampleSets",
                column: "AgeRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_CollectionId",
                table: "CollectionSampleSets",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_DonorCountId",
                table: "CollectionSampleSets",
                column: "DonorCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_SexId",
                table: "CollectionSampleSets",
                column: "SexId");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountryId",
                table: "Counties",
                column: "CountryId");

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
                name: "IX_DiagnosisCapabilities_OntologyTermId",
                table: "DiagnosisCapabilities",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisCapabilities_OrganisationId",
                table: "DiagnosisCapabilities",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisCapabilities_SampleCollectionModeId",
                table: "DiagnosisCapabilities",
                column: "SampleCollectionModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Errors_SubmissionId",
                table: "Errors",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FunderOrganisation_OrganisationsOrganisationId",
                table: "FunderOrganisation",
                column: "OrganisationsOrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_CollectionPercentageId",
                table: "MaterialDetails",
                column: "CollectionPercentageId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_MacroscopicAssessmentId",
                table: "MaterialDetails",
                column: "MacroscopicAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_MaterialTypeId",
                table: "MaterialDetails",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_PreservationTypeId",
                table: "MaterialDetails",
                column: "PreservationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_StorageTemperatureId",
                table: "MaterialDetails",
                column: "StorageTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypeMaterialTypeGroup_MaterialTypesId",
                table: "MaterialTypeMaterialTypeGroup",
                column: "MaterialTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_Networks_SopStatusId",
                table: "Networks",
                column: "SopStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyTerms_SnomedTagId",
                table: "OntologyTerms",
                column: "SnomedTagId");

            migrationBuilder.CreateIndex(
                name: "IX_OntologyVersions_OntologyId",
                table: "OntologyVersions",
                column: "OntologyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationAnnualStatistics_AnnualStatisticId",
                table: "OrganisationAnnualStatistics",
                column: "AnnualStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationNetworks_NetworkId",
                table: "OrganisationNetworks",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRegisterRequests_OrganisationTypeId",
                table: "OrganisationRegisterRequests",
                column: "OrganisationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRegistrationReasons_RegistrationReasonId",
                table: "OrganisationRegistrationReasons",
                column: "RegistrationReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_CountryId",
                table: "Organisations",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_CountyId",
                table: "Organisations",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_OrganisationTypeId",
                table: "Organisations",
                column: "OrganisationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationServiceOfferings_ServiceOfferingId",
                table: "OrganisationServiceOfferings",
                column: "ServiceOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationTypes_StorageTemperatureId",
                table: "PreservationTypes",
                column: "StorageTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_OrganisationId",
                table: "Publications",
                column: "OrganisationId");

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
                name: "IX_Samples_PreservationTypeId",
                table: "Samples",
                column: "PreservationTypeId");

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
                name: "IX_StagedSamples_PreservationTypeId",
                table: "StagedSamples",
                column: "PreservationTypeId");

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
                name: "AnnotationPublication");

            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "CapabilityAssociatedDatas");

            migrationBuilder.DropTable(
                name: "CollectionAssociatedDatas");

            migrationBuilder.DropTable(
                name: "CollectionConsentRestriction");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "FunderOrganisation");

            migrationBuilder.DropTable(
                name: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "MaterialTypeMaterialTypeGroup");

            migrationBuilder.DropTable(
                name: "NetworkRegisterRequests");

            migrationBuilder.DropTable(
                name: "NetworkUsers");

            migrationBuilder.DropTable(
                name: "OrganisationAnnualStatistics");

            migrationBuilder.DropTable(
                name: "OrganisationNetworks");

            migrationBuilder.DropTable(
                name: "OrganisationRegisterRequests");

            migrationBuilder.DropTable(
                name: "OrganisationRegistrationReasons");

            migrationBuilder.DropTable(
                name: "OrganisationServiceOfferings");

            migrationBuilder.DropTable(
                name: "OrganisationUsers");

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
                name: "TokenIssueRecords");

            migrationBuilder.DropTable(
                name: "TokenValidationRecords");

            migrationBuilder.DropTable(
                name: "Treatments");

            migrationBuilder.DropTable(
                name: "Annotations");

            migrationBuilder.DropTable(
                name: "Publications");

            migrationBuilder.DropTable(
                name: "DiagnosisCapabilities");

            migrationBuilder.DropTable(
                name: "AssociatedDataProcurementTimeframes");

            migrationBuilder.DropTable(
                name: "AssociatedDataTypes");

            migrationBuilder.DropTable(
                name: "ConsentRestrictions");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Funders");

            migrationBuilder.DropTable(
                name: "CollectionPercentages");

            migrationBuilder.DropTable(
                name: "CollectionSampleSets");

            migrationBuilder.DropTable(
                name: "MacroscopicAssessments");

            migrationBuilder.DropTable(
                name: "MaterialTypeGroups");

            migrationBuilder.DropTable(
                name: "AnnualStatistics");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "RegistrationReasons");

            migrationBuilder.DropTable(
                name: "ServiceOfferings");

            migrationBuilder.DropTable(
                name: "MaterialTypes");

            migrationBuilder.DropTable(
                name: "PreservationTypes");

            migrationBuilder.DropTable(
                name: "SampleContentMethods");

            migrationBuilder.DropTable(
                name: "OntologyVersions");

            migrationBuilder.DropTable(
                name: "TreatmentLocations");

            migrationBuilder.DropTable(
                name: "SampleCollectionModes");

            migrationBuilder.DropTable(
                name: "AssociatedDataTypeGroups");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "AgeRanges");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "DonorCounts");

            migrationBuilder.DropTable(
                name: "Sexes");

            migrationBuilder.DropTable(
                name: "AnnualStatisticGroups");

            migrationBuilder.DropTable(
                name: "SopStatus");

            migrationBuilder.DropTable(
                name: "StorageTemperatures");

            migrationBuilder.DropTable(
                name: "Ontologies");

            migrationBuilder.DropTable(
                name: "AccessConditions");

            migrationBuilder.DropTable(
                name: "CollectionPoints");

            migrationBuilder.DropTable(
                name: "CollectionStatus");

            migrationBuilder.DropTable(
                name: "CollectionTypes");

            migrationBuilder.DropTable(
                name: "HtaStatus");

            migrationBuilder.DropTable(
                name: "OntologyTerms");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "SnomedTags");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "OrganisationTypes");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
