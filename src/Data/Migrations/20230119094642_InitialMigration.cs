using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Biobanks.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgeRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LowerBound = table.Column<string>(type: "text", nullable: true),
                    UpperBound = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Annotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualStatisticGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualStatisticGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecretHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataProcurementTimeframes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayValue = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataProcurementTimeframes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContentDisposition = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionPercentages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LowerBound = table.Column<decimal>(type: "numeric", nullable: false),
                    UpperBound = table.Column<decimal>(type: "numeric", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionPercentages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsFeatureFlag = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "ConsentRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RouteSlug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryAnalyticEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventCategory = table.Column<string>(type: "text", nullable: true),
                    EventAction = table.Column<string>(type: "text", nullable: true),
                    Biobank = table.Column<string>(type: "text", nullable: true),
                    Segment = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Hostname = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Counts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryAnalyticEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryAnalyticMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PagePath = table.Column<string>(type: "text", nullable: true),
                    PagePathLevel1 = table.Column<string>(type: "text", nullable: true),
                    Segment = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Hostname = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Sessions = table.Column<int>(type: "integer", nullable: false),
                    BounceRate = table.Column<int>(type: "integer", nullable: false),
                    PercentNewSessions = table.Column<int>(type: "integer", nullable: false),
                    AvgSessionDuration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryAnalyticMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonorCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LowerBound = table.Column<int>(type: "integer", nullable: true),
                    UpperBound = table.Column<int>(type: "integer", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonorCounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MacroscopicAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroscopicAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetworkRegisterRequests",
                columns: table => new
                {
                    NetworkRegisterRequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    NetworkName = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NetworkCreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeclinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkRegisterRequests", x => x.NetworkRegisterRequestId);
                });

            migrationBuilder.CreateTable(
                name: "Ontologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ontologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PagePath = table.Column<string>(type: "text", nullable: true),
                    PreviousPagePath = table.Column<string>(type: "text", nullable: true),
                    Segment = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Hostname = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Counts = table.Column<int>(type: "integer", nullable: false),
                    OrganisationExternalId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAnalytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationTypes",
                columns: table => new
                {
                    OrganisationTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationTypes", x => x.OrganisationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationDomainRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RuleType = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationDomainRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleCollectionModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleCollectionModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleContentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleContentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOfferings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOfferings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SnomedTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnomedTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SopStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SopStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedDiagnosisDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedDiagnosisDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedSampleDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedSampleDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StagedTreatmentDeletes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedTreatmentDeletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageTemperatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageTemperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenIssueRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenIssueRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenValidationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidationSuccessful = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenValidationRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnualStatisticGroupId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualStatistics_AnnualStatisticGroups_AnnualStatisticGroup~",
                        column: x => x.AnnualStatisticGroupId,
                        principalTable: "AnnualStatisticGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: true),
                    AssociatedDataTypeGroupId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociatedDataTypes_AssociatedDataTypeGroups_AssociatedData~",
                        column: x => x.AssociatedDataTypeGroupId,
                        principalTable: "AssociatedDataTypeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Counties_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypeMaterialTypeGroup",
                columns: table => new
                {
                    MaterialTypeGroupsId = table.Column<int>(type: "integer", nullable: false),
                    MaterialTypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeMaterialTypeGroup", x => new { x.MaterialTypeGroupsId, x.MaterialTypesId });
                    table.ForeignKey(
                        name: "FK_MaterialTypeMaterialTypeGroup_MaterialTypeGroups_MaterialTy~",
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OntologyId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
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
                    OrganisationRegisterRequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    OrganisationTypeId = table.Column<int>(type: "integer", nullable: false),
                    OrganisationName = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganisationCreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeclinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganisationExternalId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRegisterRequests", x => x.OrganisationRegisterRequestId);
                    table.ForeignKey(
                        name: "FK_OrganisationRegisterRequests_OrganisationTypes_Organisation~",
                        column: x => x.OrganisationTypeId,
                        principalTable: "OrganisationTypes",
                        principalColumn: "OrganisationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTerms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    OtherTerms = table.Column<string>(type: "text", nullable: true),
                    SnomedTagId = table.Column<int>(type: "integer", nullable: true),
                    DisplayOnDirectory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OntologyTerms_SnomedTags_SnomedTagId",
                        column: x => x.SnomedTagId,
                        principalTable: "SnomedTags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    NetworkId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SopStatusId = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContactHandoverEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    HandoverBaseUrl = table.Column<string>(type: "text", nullable: true),
                    HandoverOrgIdsUrlParamName = table.Column<string>(type: "text", nullable: true),
                    MultipleHandoverOrdIdsParams = table.Column<bool>(type: "boolean", nullable: false),
                    HandoverNonMembers = table.Column<bool>(type: "boolean", nullable: false),
                    HandoverNonMembersUrlParamName = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BiobankId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalRecords = table.Column<int>(type: "integer", nullable: false),
                    RecordsProcessed = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    StatusChangeTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StorageTemperatureId = table.Column<int>(type: "integer", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    OrganisationExternalId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: true),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressLine3 = table.Column<string>(type: "text", nullable: true),
                    AddressLine4 = table.Column<string>(type: "text", nullable: true),
                    PostCode = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    CountyId = table.Column<int>(type: "integer", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    OrganisationTypeId = table.Column<int>(type: "integer", nullable: false),
                    GoverningInstitution = table.Column<string>(type: "text", nullable: true),
                    GoverningDepartment = table.Column<string>(type: "text", nullable: true),
                    SharingOptOut = table.Column<bool>(type: "boolean", nullable: false),
                    EthicsRegistration = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AnonymousIdentifier = table.Column<Guid>(type: "uuid", nullable: true),
                    OtherRegistrationReason = table.Column<string>(type: "text", nullable: true),
                    ExcludePublications = table.Column<bool>(type: "boolean", nullable: false),
                    AccessConditionId = table.Column<int>(type: "integer", nullable: true),
                    CollectionTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.OrganisationId);
                    table.ForeignKey(
                        name: "FK_Organisations_AccessConditions_AccessConditionId",
                        column: x => x.AccessConditionId,
                        principalTable: "AccessConditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organisations_CollectionTypes_CollectionTypeId",
                        column: x => x.CollectionTypeId,
                        principalTable: "CollectionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organisations_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organisations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organisations_OrganisationTypes_OrganisationTypeId",
                        column: x => x.OrganisationTypeId,
                        principalTable: "OrganisationTypes",
                        principalColumn: "OrganisationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataTypeOntologyTerm",
                columns: table => new
                {
                    AssociatedDataTypesId = table.Column<int>(type: "integer", nullable: false),
                    OntologyTermsId = table.Column<string>(type: "character varying(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypeOntologyTerm", x => new { x.AssociatedDataTypesId, x.OntologyTermsId });
                    table.ForeignKey(
                        name: "FK_AssociatedDataTypeOntologyTerm_AssociatedDataTypes_Associat~",
                        column: x => x.AssociatedDataTypesId,
                        principalTable: "AssociatedDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociatedDataTypeOntologyTerm_OntologyTerms_OntologyTermsId",
                        column: x => x.OntologyTermsId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateDiagnosed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiagnosisCodeId = table.Column<string>(type: "character varying(20)", nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(type: "integer", nullable: false)
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
                name: "MaterialTypeOntologyTerm",
                columns: table => new
                {
                    ExtractionProceduresId = table.Column<string>(type: "character varying(20)", nullable: false),
                    MaterialTypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeOntologyTerm", x => new { x.ExtractionProceduresId, x.MaterialTypesId });
                    table.ForeignKey(
                        name: "FK_MaterialTypeOntologyTerm_MaterialTypes_MaterialTypesId",
                        column: x => x.MaterialTypesId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialTypeOntologyTerm_OntologyTerms_ExtractionProcedures~",
                        column: x => x.ExtractionProceduresId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StagedDiagnoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateDiagnosed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiagnosisCodeId = table.Column<string>(type: "character varying(20)", nullable: false),
                    DiagnosisCodeOntologyVersionId = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_StagedDiagnoses_OntologyVersions_DiagnosisCodeOntologyVersi~",
                        column: x => x.DiagnosisCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StagedTreatments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateTreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TreatmentCodeId = table.Column<string>(type: "character varying(20)", nullable: false),
                    TreatmentLocationId = table.Column<int>(type: "integer", nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_StagedTreatments_OntologyVersions_TreatmentCodeOntologyVers~",
                        column: x => x.TreatmentCodeOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StagedTreatments_TreatmentLocations_TreatmentLocationId",
                        column: x => x.TreatmentLocationId,
                        principalTable: "TreatmentLocations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Treatments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateTreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TreatmentCodeId = table.Column<string>(type: "character varying(20)", nullable: false),
                    TreatmentLocationId = table.Column<int>(type: "integer", nullable: true),
                    TreatmentCodeOntologyVersionId = table.Column<int>(type: "integer", nullable: false)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NetworkUsers",
                columns: table => new
                {
                    NetworkId = table.Column<int>(type: "integer", nullable: false),
                    NetworkUserId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: true),
                    RecordIdentifiers = table.Column<string>(type: "text", nullable: true),
                    SubmissionId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDirty = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Barcode = table.Column<string>(type: "text", nullable: false),
                    YearOfBirth = table.Column<int>(type: "integer", nullable: true),
                    AgeAtDonation = table.Column<string>(type: "text", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "integer", nullable: true),
                    PreservationTypeId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(type: "character varying(20)", nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(type: "integer", nullable: true),
                    ExtractionProcedureId = table.Column<string>(type: "character varying(20)", nullable: true),
                    SampleContentId = table.Column<string>(type: "character varying(20)", nullable: true),
                    SampleContentMethodId = table.Column<int>(type: "integer", nullable: true),
                    SexId = table.Column<int>(type: "integer", nullable: true),
                    CollectionName = table.Column<string>(type: "text", nullable: true)
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_OntologyTerms_ExtractionSiteId",
                        column: x => x.ExtractionSiteId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_OntologyTerms_SampleContentId",
                        column: x => x.SampleContentId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_OntologyVersions_ExtractionSiteOntologyVersionId",
                        column: x => x.ExtractionSiteOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_PreservationTypes_PreservationTypeId",
                        column: x => x.PreservationTypeId,
                        principalTable: "PreservationTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_SampleContentMethods_SampleContentMethodId",
                        column: x => x.SampleContentMethodId,
                        principalTable: "SampleContentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StagedSamples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IndividualReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Barcode = table.Column<string>(type: "text", nullable: false),
                    YearOfBirth = table.Column<int>(type: "integer", nullable: true),
                    AgeAtDonation = table.Column<string>(type: "text", nullable: true),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "integer", nullable: true),
                    PreservationTypeId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "date", nullable: false),
                    ExtractionSiteId = table.Column<string>(type: "character varying(20)", nullable: true),
                    ExtractionSiteOntologyVersionId = table.Column<int>(type: "integer", nullable: true),
                    ExtractionProcedureId = table.Column<string>(type: "character varying(20)", nullable: true),
                    SampleContentId = table.Column<string>(type: "character varying(20)", nullable: true),
                    SampleContentMethodId = table.Column<int>(type: "integer", nullable: true),
                    SexId = table.Column<int>(type: "integer", nullable: true),
                    CollectionName = table.Column<string>(type: "text", nullable: true)
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyTerms_ExtractionSiteId",
                        column: x => x.ExtractionSiteId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyTerms_SampleContentId",
                        column: x => x.SampleContentId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_OntologyVersions_ExtractionSiteOntologyVersio~",
                        column: x => x.ExtractionSiteOntologyVersionId,
                        principalTable: "OntologyVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_PreservationTypes_PreservationTypeId",
                        column: x => x.PreservationTypeId,
                        principalTable: "PreservationTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_SampleContentMethods_SampleContentMethodId",
                        column: x => x.SampleContentMethodId,
                        principalTable: "SampleContentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StagedSamples_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApiClientOrganisation",
                columns: table => new
                {
                    ApiClientsId = table.Column<int>(type: "integer", nullable: false),
                    OrganisationsOrganisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClientOrganisation", x => new { x.ApiClientsId, x.OrganisationsOrganisationId });
                    table.ForeignKey(
                        name: "FK_ApiClientOrganisation_ApiClients_ApiClientsId",
                        column: x => x.ApiClientsId,
                        principalTable: "ApiClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiClientOrganisation_Organisations_OrganisationsOrganisati~",
                        column: x => x.OrganisationsOrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    OntologyTermId = table.Column<string>(type: "character varying(20)", nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FromApi = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    AccessConditionId = table.Column<int>(type: "integer", nullable: false),
                    CollectionTypeId = table.Column<int>(type: "integer", nullable: true),
                    CollectionStatusId = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_Collections_CollectionStatus_CollectionStatusId",
                        column: x => x.CollectionStatusId,
                        principalTable: "CollectionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collections_CollectionTypes_CollectionTypeId",
                        column: x => x.CollectionTypeId,
                        principalTable: "CollectionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Collections_OntologyTerms_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
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
                    DiagnosisCapabilityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    OntologyTermId = table.Column<string>(type: "character varying(20)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SampleCollectionModeId = table.Column<int>(type: "integer", nullable: false),
                    AnnualDonorExpectation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosisCapabilities", x => x.DiagnosisCapabilityId);
                    table.ForeignKey(
                        name: "FK_DiagnosisCapabilities_OntologyTerms_OntologyTermId",
                        column: x => x.OntologyTermId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiagnosisCapabilities_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagnosisCapabilities_SampleCollectionModes_SampleCollectio~",
                        column: x => x.SampleCollectionModeId,
                        principalTable: "SampleCollectionModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunderOrganisation",
                columns: table => new
                {
                    FundersId = table.Column<int>(type: "integer", nullable: false),
                    OrganisationsOrganisationId = table.Column<int>(type: "integer", nullable: false)
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
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    AnnualStatisticId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAnnualStatistics", x => new { x.OrganisationId, x.AnnualStatisticId, x.Year });
                    table.ForeignKey(
                        name: "FK_OrganisationAnnualStatistics_AnnualStatistics_AnnualStatist~",
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
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    NetworkId = table.Column<int>(type: "integer", nullable: false),
                    ExternalID = table.Column<string>(type: "text", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    RegistrationReasonId = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_OrganisationRegistrationReasons_RegistrationReasons_Registr~",
                        column: x => x.RegistrationReasonId,
                        principalTable: "RegistrationReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationServiceOfferings",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    ServiceOfferingId = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_OrganisationServiceOfferings_ServiceOfferings_ServiceOfferi~",
                        column: x => x.ServiceOfferingId,
                        principalTable: "ServiceOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUsers",
                columns: table => new
                {
                    OrganisationId = table.Column<int>(type: "integer", nullable: false),
                    OrganisationUserId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicationId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Authors = table.Column<string>(type: "text", nullable: true),
                    Journal = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    DOI = table.Column<string>(type: "text", nullable: true),
                    Accepted = table.Column<bool>(type: "boolean", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    AnnotationsSynced = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganisationId = table.Column<int>(type: "integer", nullable: false)
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
                    CollectionId = table.Column<int>(type: "integer", nullable: false),
                    AssociatedDataTypeId = table.Column<int>(type: "integer", nullable: false),
                    AssociatedDataProcurementTimeframeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionAssociatedDatas", x => new { x.CollectionId, x.AssociatedDataTypeId });
                    table.ForeignKey(
                        name: "FK_CollectionAssociatedDatas_AssociatedDataProcurementTimefram~",
                        column: x => x.AssociatedDataProcurementTimeframeId,
                        principalTable: "AssociatedDataProcurementTimeframes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CollectionAssociatedDatas_AssociatedDataTypes_AssociatedDat~",
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
                    CollectionsCollectionId = table.Column<int>(type: "integer", nullable: false),
                    ConsentRestrictionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionConsentRestriction", x => new { x.CollectionsCollectionId, x.ConsentRestrictionsId });
                    table.ForeignKey(
                        name: "FK_CollectionConsentRestriction_Collections_CollectionsCollect~",
                        column: x => x.CollectionsCollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionConsentRestriction_ConsentRestrictions_ConsentRes~",
                        column: x => x.ConsentRestrictionsId,
                        principalTable: "ConsentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SampleSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CollectionId = table.Column<int>(type: "integer", nullable: false),
                    SexId = table.Column<int>(type: "integer", nullable: false),
                    AgeRangeId = table.Column<int>(type: "integer", nullable: false),
                    DonorCountId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleSets_AgeRanges_AgeRangeId",
                        column: x => x.AgeRangeId,
                        principalTable: "AgeRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleSets_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleSets_DonorCounts_DonorCountId",
                        column: x => x.DonorCountId,
                        principalTable: "DonorCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleSets_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CapabilityAssociatedDatas",
                columns: table => new
                {
                    DiagnosisCapabilityId = table.Column<int>(type: "integer", nullable: false),
                    AssociatedDataTypeId = table.Column<int>(type: "integer", nullable: false),
                    AssociatedDataProcurementTimeframeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapabilityAssociatedDatas", x => new { x.DiagnosisCapabilityId, x.AssociatedDataTypeId });
                    table.ForeignKey(
                        name: "FK_CapabilityAssociatedDatas_AssociatedDataProcurementTimefram~",
                        column: x => x.AssociatedDataProcurementTimeframeId,
                        principalTable: "AssociatedDataProcurementTimeframes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CapabilityAssociatedDatas_AssociatedDataTypes_AssociatedDat~",
                        column: x => x.AssociatedDataTypeId,
                        principalTable: "AssociatedDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CapabilityAssociatedDatas_DiagnosisCapabilities_DiagnosisCa~",
                        column: x => x.DiagnosisCapabilityId,
                        principalTable: "DiagnosisCapabilities",
                        principalColumn: "DiagnosisCapabilityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnotationPublication",
                columns: table => new
                {
                    AnnotationsId = table.Column<int>(type: "integer", nullable: false),
                    PublicationsId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SampleSetId = table.Column<int>(type: "integer", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    StorageTemperatureId = table.Column<int>(type: "integer", nullable: false),
                    MacroscopicAssessmentId = table.Column<int>(type: "integer", nullable: false),
                    ExtractionProcedureId = table.Column<string>(type: "character varying(20)", nullable: true),
                    PreservationTypeId = table.Column<int>(type: "integer", nullable: true),
                    CollectionPercentageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_CollectionPercentages_CollectionPercentageId",
                        column: x => x.CollectionPercentageId,
                        principalTable: "CollectionPercentages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialDetails_MacroscopicAssessments_MacroscopicAssessmen~",
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
                        name: "FK_MaterialDetails_OntologyTerms_ExtractionProcedureId",
                        column: x => x.ExtractionProcedureId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                        column: x => x.PreservationTypeId,
                        principalTable: "PreservationTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialDetails_SampleSets_SampleSetId",
                        column: x => x.SampleSetId,
                        principalTable: "SampleSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialDetails_StorageTemperatures_StorageTemperatureId",
                        column: x => x.StorageTemperatureId,
                        principalTable: "StorageTemperatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeRanges_LowerBound_UpperBound",
                table: "AgeRanges",
                columns: new[] { "LowerBound", "UpperBound" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationPublication_PublicationsId",
                table: "AnnotationPublication",
                column: "PublicationsId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualStatistics_AnnualStatisticGroupId",
                table: "AnnualStatistics",
                column: "AnnualStatisticGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClientOrganisation_OrganisationsOrganisationId",
                table: "ApiClientOrganisation",
                column: "OrganisationsOrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssociatedDataTypeOntologyTerm_OntologyTermsId",
                table: "AssociatedDataTypeOntologyTerm",
                column: "OntologyTermsId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociatedDataTypes_AssociatedDataTypeGroupId",
                table: "AssociatedDataTypes",
                column: "AssociatedDataTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityAssociatedDatas_AssociatedDataProcurementTimefram~",
                table: "CapabilityAssociatedDatas",
                column: "AssociatedDataProcurementTimeframeId");

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityAssociatedDatas_AssociatedDataTypeId",
                table: "CapabilityAssociatedDatas",
                column: "AssociatedDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionAssociatedDatas_AssociatedDataProcurementTimefram~",
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
                name: "IX_Collections_CollectionStatusId",
                table: "Collections",
                column: "CollectionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CollectionTypeId",
                table: "Collections",
                column: "CollectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_OntologyTermId",
                table: "Collections",
                column: "OntologyTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_OrganisationId",
                table: "Collections",
                column: "OrganisationId");

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
                name: "IX_Diagnoses_OrganisationId_IndividualReferenceId_DateDiagnose~",
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
                name: "IX_MaterialDetails_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId");

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
                name: "IX_MaterialDetails_SampleSetId_MaterialTypeId_StorageTemperatu~",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "ExtractionProcedureId", "PreservationTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_StorageTemperatureId",
                table: "MaterialDetails",
                column: "StorageTemperatureId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypeMaterialTypeGroup_MaterialTypesId",
                table: "MaterialTypeMaterialTypeGroup",
                column: "MaterialTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypeOntologyTerm_MaterialTypesId",
                table: "MaterialTypeOntologyTerm",
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
                name: "IX_Organisations_AccessConditionId",
                table: "Organisations",
                column: "AccessConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_CollectionTypeId",
                table: "Organisations",
                column: "CollectionTypeId");

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
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_Collec~",
                table: "Samples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true);

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
                name: "IX_SampleSets_AgeRangeId",
                table: "SampleSets",
                column: "AgeRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_CollectionId",
                table: "SampleSets",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_DonorCountId",
                table: "SampleSets",
                column: "DonorCountId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_SexId",
                table: "SampleSets",
                column: "SexId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_DiagnosisCodeId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_DiagnosisCodeOntologyVersionId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeOntologyVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_StagedDiagnoses_OrganisationId_IndividualReferenceId_DateDi~",
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
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_~",
                table: "StagedSamples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true);

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
                name: "IX_StagedTreatments_OrganisationId_IndividualReferenceId_DateT~",
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
                name: "IX_Treatments_OrganisationId_IndividualReferenceId_DateTreated~",
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
                name: "ApiClientOrganisation");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssociatedDataTypeOntologyTerm");

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
                name: "ContentPages");

            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "DirectoryAnalyticEvents");

            migrationBuilder.DropTable(
                name: "DirectoryAnalyticMetrics");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "FunderOrganisation");

            migrationBuilder.DropTable(
                name: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "MaterialTypeMaterialTypeGroup");

            migrationBuilder.DropTable(
                name: "MaterialTypeOntologyTerm");

            migrationBuilder.DropTable(
                name: "NetworkRegisterRequests");

            migrationBuilder.DropTable(
                name: "NetworkUsers");

            migrationBuilder.DropTable(
                name: "OrganisationAnalytics");

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
                name: "RegistrationDomainRules");

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
                name: "ApiClients");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

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
                name: "MacroscopicAssessments");

            migrationBuilder.DropTable(
                name: "SampleSets");

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
                name: "CollectionStatus");

            migrationBuilder.DropTable(
                name: "OntologyTerms");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "SnomedTags");

            migrationBuilder.DropTable(
                name: "AccessConditions");

            migrationBuilder.DropTable(
                name: "CollectionTypes");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "OrganisationTypes");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
