using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Migrations
{
    public partial class RefData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessConditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgeRanges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualStatisticGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualStatisticGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataProcurementTimeframes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataProcurementTimeframes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedDataTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionPercentages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionPercentages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsentRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonorCounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonorCounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HtaStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtaStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MacroscopicAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroscopicAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OntologyTerms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Tag = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologyTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOfferings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOfferings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sexes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SopStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SopStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageTemperatures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageTemperatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    annualStatisticGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualStatistics_AnnualStatisticGroup_annualStatisticGroupId",
                        column: x => x.annualStatisticGroupId,
                        principalTable: "AnnualStatisticGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(nullable: false),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Counties_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypeGroupMaterialType",
                columns: table => new
                {
                    MaterialTypeId = table.Column<int>(nullable: false),
                    MaterialTypeGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeGroupMaterialType", x => new { x.MaterialTypeId, x.MaterialTypeGroupId });
                    table.ForeignKey(
                        name: "FK_MaterialTypeGroupMaterialType_MaterialTypeGroups_MaterialTypeGroupId",
                        column: x => x.MaterialTypeGroupId,
                        principalTable: "MaterialTypeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialTypeGroupMaterialType_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualStatistics_annualStatisticGroupId",
                table: "AnnualStatistics",
                column: "annualStatisticGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountryId",
                table: "Counties",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypeGroupMaterialType_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType",
                column: "MaterialTypeGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessConditions");

            migrationBuilder.DropTable(
                name: "AgeRanges");

            migrationBuilder.DropTable(
                name: "AnnualStatistics");

            migrationBuilder.DropTable(
                name: "AssociatedDataProcurementTimeframes");

            migrationBuilder.DropTable(
                name: "AssociatedDataTypes");

            migrationBuilder.DropTable(
                name: "CollectionPercentages");

            migrationBuilder.DropTable(
                name: "CollectionPoints");

            migrationBuilder.DropTable(
                name: "CollectionStatuses");

            migrationBuilder.DropTable(
                name: "CollectionTypes");

            migrationBuilder.DropTable(
                name: "ConsentRestrictions");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "DonorCounts");

            migrationBuilder.DropTable(
                name: "Funders");

            migrationBuilder.DropTable(
                name: "HtaStatuses");

            migrationBuilder.DropTable(
                name: "MacroscopicAssessments");

            migrationBuilder.DropTable(
                name: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropTable(
                name: "OntologyTerms");

            migrationBuilder.DropTable(
                name: "ServiceOfferings");

            migrationBuilder.DropTable(
                name: "Sexes");

            migrationBuilder.DropTable(
                name: "SopStatuses");

            migrationBuilder.DropTable(
                name: "StorageTemperatures");

            migrationBuilder.DropTable(
                name: "AnnualStatisticGroup");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "MaterialTypeGroups");

            migrationBuilder.DropTable(
                name: "MaterialTypes");
        }
    }
}
