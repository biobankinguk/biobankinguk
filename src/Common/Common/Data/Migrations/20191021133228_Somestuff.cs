using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Migrations
{
    public partial class Somestuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnualStatistics_AnnualStatisticGroup_AnnualStatisticGroupId",
                table: "AnnualStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialType",
                table: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnualStatisticGroup",
                table: "AnnualStatisticGroup");

            migrationBuilder.RenameTable(
                name: "MaterialTypeGroupMaterialType",
                newName: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.RenameTable(
                name: "AnnualStatisticGroup",
                newName: "AnnualStatisticGroups");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialTypeGroupMaterialType_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialTypes",
                newName: "IX_MaterialTypeGroupMaterialTypes_MaterialTypeGroupId");

            migrationBuilder.AddColumn<int>(
                name: "LowerBound",
                table: "DonorCounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpperBound",
                table: "DonorCounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialTypes",
                table: "MaterialTypeGroupMaterialTypes",
                columns: new[] { "MaterialTypeId", "MaterialTypeGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnualStatisticGroups",
                table: "AnnualStatisticGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnualStatistics_AnnualStatisticGroups_AnnualStatisticGroupId",
                table: "AnnualStatistics",
                column: "AnnualStatisticGroupId",
                principalTable: "AnnualStatisticGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialTypes",
                column: "MaterialTypeGroupId",
                principalTable: "MaterialTypeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialTypes",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnualStatistics_AnnualStatisticGroups_AnnualStatisticGroupId",
                table: "AnnualStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialTypes",
                table: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnualStatisticGroups",
                table: "AnnualStatisticGroups");

            migrationBuilder.DropColumn(
                name: "LowerBound",
                table: "DonorCounts");

            migrationBuilder.DropColumn(
                name: "UpperBound",
                table: "DonorCounts");

            migrationBuilder.RenameTable(
                name: "MaterialTypeGroupMaterialTypes",
                newName: "MaterialTypeGroupMaterialType");

            migrationBuilder.RenameTable(
                name: "AnnualStatisticGroups",
                newName: "AnnualStatisticGroup");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialTypeGroupMaterialTypes_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType",
                newName: "IX_MaterialTypeGroupMaterialType_MaterialTypeGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialType",
                table: "MaterialTypeGroupMaterialType",
                columns: new[] { "MaterialTypeId", "MaterialTypeGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnualStatisticGroup",
                table: "AnnualStatisticGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnualStatistics_AnnualStatisticGroup_AnnualStatisticGroupId",
                table: "AnnualStatistics",
                column: "AnnualStatisticGroupId",
                principalTable: "AnnualStatisticGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType",
                column: "MaterialTypeGroupId",
                principalTable: "MaterialTypeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialType",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
