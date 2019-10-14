using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Data.Migrations
{
    public partial class thiny : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroups_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroups");

            migrationBuilder.DropIndex(
                name: "IX_MaterialTypeGroups_MaterialTypeId",
                table: "MaterialTypeGroups");

            migrationBuilder.DropColumn(
                name: "MaterialTypeId",
                table: "MaterialTypeGroups");

            migrationBuilder.DropColumn(
                name: "LowerBound",
                table: "DonorCounts");

            migrationBuilder.DropColumn(
                name: "UpperBound",
                table: "DonorCounts");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "AnnualStatistics");

            migrationBuilder.AddColumn<int>(
                name: "annualStatisticGroupId",
                table: "AnnualStatistics",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_MaterialTypeGroupMaterialType_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType",
                column: "MaterialTypeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnualStatistics_AnnualStatisticGroup_annualStatisticGroupId",
                table: "AnnualStatistics",
                column: "annualStatisticGroupId",
                principalTable: "AnnualStatisticGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnualStatistics_AnnualStatisticGroup_annualStatisticGroupId",
                table: "AnnualStatistics");

            migrationBuilder.DropTable(
                name: "AnnualStatisticGroup");

            migrationBuilder.DropTable(
                name: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropIndex(
                name: "IX_AnnualStatistics_annualStatisticGroupId",
                table: "AnnualStatistics");

            migrationBuilder.DropColumn(
                name: "annualStatisticGroupId",
                table: "AnnualStatistics");

            migrationBuilder.AddColumn<int>(
                name: "MaterialTypeId",
                table: "MaterialTypeGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LowerBound",
                table: "DonorCounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpperBound",
                table: "DonorCounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "AnnualStatistics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypeGroups_MaterialTypeId",
                table: "MaterialTypeGroups",
                column: "MaterialTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroups_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroups",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
